using System;
using System.Threading.Tasks;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.Exceptions;
using AuthCraft.Common.Extensions;
using AuthCraft.Common.ServiceInterfaces;
using Microsoft.Extensions.Logging;

namespace AuthCraft.Services;

public class ClientAuthenticationService : IClientAuthenticationService
{
    private readonly ILogger _logger;
    private readonly IConfigCrestConfigurationService _configCrestConfigurationService;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly string _clientsListConfigKey = "clients";

    public ClientAuthenticationService(
        ILogger<ClientAuthenticationService> logger,
        IConfigCrestConfigurationService configCrestConfigurationService,
        ISecurityTokenService securityTokenService)
    {
        _logger = logger;
        _configCrestConfigurationService = configCrestConfigurationService;
        _securityTokenService = securityTokenService;
    }

    public async Task<ClientAuthenticationOutput> AuthenticateAsync(Guid key)
    {
        _logger.LogTrace($"Calling {nameof(ClientAuthenticationService)} with Key={key}.");

        var clients = await _configCrestConfigurationService.GetConfigItemAsync<ClientList>(_clientsListConfigKey);
        if (clients == null || clients.Empty())
        {
            throw new ConfigurationMissingException($"No clients found. Please check ConfigurationSetId={ClientList.SectionName}");
        }

        var client = clients.Get(key);
        if (client == null)
        {
            throw new UnauthorizedException("Unable to find client for the specified key");
        }

        _logger.LogTrace("Client for the specified key has been found");

        var tokenOutput = await _securityTokenService.CreateAsync(client);

        return new ClientAuthenticationOutput
        {
            Application = client.Application,
            Token = tokenOutput.Token,
            ExpiresAt = tokenOutput.ExpiresAt
        };
    }
}
