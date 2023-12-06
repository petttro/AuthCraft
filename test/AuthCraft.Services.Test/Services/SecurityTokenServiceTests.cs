using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AuthCraft.App.Services;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.Infrastructure.Aws;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using AuthCraft.Data.Dto;
using Xunit;

namespace AuthCraft.Services.Tests.Services;

public class SecurityTokenServiceTest
{
    private readonly SecurityTokenService _securityTokenProvider;
    private readonly Mock<ISecretsService> _secretsServiceMock;

    // this private JWK comes from RSA private key found in ConvertPemToJwkTests
    private readonly string _privateJwk =
        "{\"alg\":\"RS256\",\"d\":\"QErmNuFiXuN-OKpQ4pQP-2sqsyw09ABgmJ-8WPOgAcaIDubf2ECVOl8O8NNs6c4n_Djx5nHVWDwnjlmbz0mzFYXVX6j89UfUIIRtwDUQThALQGlc-lEF-3vr7aSnjtbA4rdJ3YOe2AOebG4gfHND2NiUFczpgd_yMY5bsIMoIfk\",\"dp\":\"K7jjod4IXKw7YUrnDGeb-sRYBJBDg1At5413ZLFXeUMBOagFkAqt6ypW659RAo2FM51CEWxh-gQ6h_-5IraWwQ\",\"dq\":\"NeifInMRIAR5Hn6sRx8C51etI93WRHHdmTODAInrMSM1Ez_1DSvq2KaZW9KN7wjyIKr8wkpOkLo-9nHcgLarjQ\",\"e\":\"AQAB\",\"kid\":\"fd5b96c2-bf4f-40e4-abee-de1480e457f9\",\"kty\":\"RSA\",\"n\":\"UBE7n9qbYvrBqStbJwsH4gyhDKNGvL5YZRGXHNeVqp53-loTo5gPbOpENgI3jNIt4oPoyHqbR1PKty14Y4mHdyF2o0sfjyfQz4LGVSmAEasF6Q-57I-GvFWcW2x6M0FtjXsBYtuyAqT-00t3eRgyq-YrmVatKunoSKiylkVLZxM\",\"p\":\"oAszZklav40KfQBZGSVpCsHQFZ-Sd7PVyXw1ywyHIRfxT4RzMIEAFWTrftS2lviPr_azke_ON-j1bCD1vWweFQ\",\"q\":\"gBKbYHo7rlAnOf-eDmVk7u4YDLkG-zG0VxgvjTIN381G8OuRbd3P-Lzi-dGULirPeu8kXwTtJwocxk3eWxXihw\",\"qi\":\"HDyRELzU4fiVxeKSSSBGOIpBWRgm-300RNXI1auPQ5_VA-inplxhvn1WC1HBm93_eyr9jSKs2ggNcOGbBwp2Mw\",\"use\":\"sig\"}";

    // this public JWK comes from RSA public key found in ConvertPemToJwkTests
    private readonly string _publicJwk =
        "{\"alg\":\"RS256\",\"e\":\"AQAB\",\"kid\":\"d514fa88-56bb-45fb-b754-bd7841d60453\",\"kty\":\"RSA\",\"n\":\"UBE7n9qbYvrBqStbJwsH4gyhDKNGvL5YZRGXHNeVqp53-loTo5gPbOpENgI3jNIt4oPoyHqbR1PKty14Y4mHdyF2o0sfjyfQz4LGVSmAEasF6Q-57I-GvFWcW2x6M0FtjXsBYtuyAqT-00t3eRgyq-YrmVatKunoSKiylkVLZxM\",\"use\":\"sig\"}";

    public SecurityTokenServiceTest()
    {
        _secretsServiceMock = new Mock<ISecretsService>();
        _securityTokenProvider = new SecurityTokenService(new NullLogger<SecurityTokenService>(), _secretsServiceMock.Object);
    }

    [Fact]
    public async Task SecurityTokenService_Create_ClientDtoValidRequestReturnsTokenAndExpirationTime()
    {
        // Arrange
        var request = new ClientList.Client
        {
            Application = "application",
            ResourceClaims = new List<ResourceClaims>(),
            TokenExpirationMinutes = 240
        };

        _secretsServiceMock
            .Setup(x => x.GetSigningKeyAsync())
            .ReturnsAsync(new SigningInfo
            {
                Audience = "Audience",
                Issuer = "AuthCraft",
                Private = _privateJwk
            });

        // Act
        var result = await _securityTokenProvider.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(ValidateToken(result.Token));
        Assert.Equal(DateTimeKind.Utc, result.ExpiresAt.Kind);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    private bool ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var publicJwk = new JsonWebKey(_publicJwk);
        var validationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = false,
            ValidIssuer = "AuthCraft",
            ValidAudience = "Audience",
            IssuerSigningKey = publicJwk,
        };

        try
        {
            SecurityToken validToken;
            handler.ValidateToken(token, validationParameters, out validToken);

            var validJwt = validToken as JwtSecurityToken;

            if (validJwt == null)
            {
                return false;
            }

            if (!validJwt.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.Ordinal))
            {
                return false;
            }

            // Additional custom validation of JWT claims here (if any)
        }
        catch (SecurityTokenValidationException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }
}
