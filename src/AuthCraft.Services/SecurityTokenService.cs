using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.Infrastructure.Aws;
using AuthCraft.Common.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AuthCraft.Services;

// This class should be always registered as singleton to avoid multiple RSA object creation
public class SecurityTokenService : ISecurityTokenService
{
    private readonly ILogger _logger;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly ISecretsService _secretsService;

    public SecurityTokenService(ILogger<SecurityTokenService> logger, ISecretsService secretsService)
    {
        _logger = logger;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        _secretsService = secretsService;
    }

    public async Task<CreateTokenOutput> CreateAsync(ClientList.Client client)
    {
        _logger.LogTrace($"Calling {nameof(SecurityTokenService)} with Client parameter: client with Application={client?.Application}.");

        var dto = new
        {
            ApplicationName = client.Application,
            TokenExpirationMinutes = client.TokenExpirationMinutes
        };

        var claims = ConvertToClaims(typeof(AuthCraftClaimTypes.Client), dto);
        var resourceClaims = CreateResourceClaims(client.ResourceClaims);
        claims = resourceClaims.Concat(claims);

        var identity = new ClientIdentity(claims);

        var expiresIn = TimeSpan.FromMinutes(client.TokenExpirationMinutes);

        return await GenerateToken(identity, expiresIn);
    }

    private static IEnumerable<Claim> CreateResourceClaims(List<ResourceClaims> resourceClaims)
    {
        var claims = new List<Claim>();

        if (resourceClaims == null)
        {
            return claims;
        }

        // Serialize into a list of Claim objects
        foreach (var resourceClaim in resourceClaims)
        {
            claims.AddRange(resourceClaim.Values.Select(entry => new Claim(resourceClaim.Key, entry)));
        }

        return claims;
    }

    /// <summary>
    /// Convert object to a set of claims using simple name matching, based on supplied type, describing claim types to convert to
    /// </summary>
    /// <param name="claimsType">Type, describing claim types</param>
    /// <param name="objectToConvert">Object, which should be converted</param>
    /// <returns></returns>
    /// TODO might want to reimplement using FastMember, once the issue is resolved https://github.com/mgravell/fast-member/issues/11
    private static IEnumerable<Claim> ConvertToClaims(Type claimsType, object objectToConvert)
    {
        var objProperties = objectToConvert
            .GetType()
            .GetTypeInfo()
            .GetProperties()
            .ToDictionary(x => x.Name, x => x.GetValue(objectToConvert)?.ToString());
        var claimsTypes = claimsType
            .GetTypeInfo()
            .GetFields()
            .ToDictionary(x => x.Name, x => x.GetValue(null).ToString());

        return claimsTypes
            .Where(x => objProperties.ContainsKey(x.Key))
            .Select(kvp =>
            {
                var propertyValue = objProperties[kvp.Key];
                return propertyValue == null ? new Claim(kvp.Value, string.Empty) : new Claim(kvp.Value, propertyValue);
            });
    }

    private async Task<CreateTokenOutput> GenerateToken(ClaimsIdentity identity, TimeSpan expiresIn)
    {
        _logger.LogTrace($"Calling {nameof(GenerateToken)} with identity with " +
                         $"AuthenticationType={identity?.AuthenticationType}, NameClaimType={identity?.NameClaimType}, and ExpiresIn={expiresIn}.");

        var now = DateTimeOffset.UtcNow;
        var expirationTime = now.UtcDateTime.Add(expiresIn);
        var nonceClaim = new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        var signingInfo = await _secretsService.GetSigningKeyAsync();
        var privateJwk = new JsonWebKey(signingInfo.Private);

        identity?.AddClaim(nonceClaim);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = signingInfo.Issuer,
            Audience = signingInfo.Audience,
            NotBefore = now.UtcDateTime,
            Expires = expirationTime,
            SigningCredentials = new SigningCredentials(privateJwk, privateJwk.Alg),
            Subject = identity
        };

        var jwt = _jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        _logger.LogTrace($"CreateJwtSecurityToken is created: jwt with Id={jwt?.Id}, Issuer={jwt?.Issuer}.");

        return new CreateTokenOutput
        {
            Token = _jwtSecurityTokenHandler.WriteToken(jwt),
            ExpiresAt = expirationTime
        };
    }
}
