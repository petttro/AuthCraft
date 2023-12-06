using System;
using System.Threading.Tasks;
using AuthCraft.Common;
using AuthCraft.Common.Exceptions;
using AuthCraft.Common.ServiceInterfaces;
using AuthCraft.Data.Dto.ConfigCrest;
using AuthCraft.Data.HttpClients;
using Microsoft.Extensions.Logging;
using AuthCraft.Common.Extensions;
using Polly;
using Polly.Registry;

namespace AuthCraft.App.Services;

public class ConfigCrestConfigurationService : IConfigCrestConfigurationService
{
    private readonly IConfigCrestHttpClient _configCrestHttpClient;
    private readonly ILogger _logger;
    private readonly IPolicyRegistry<string> _policyRegistry;

    public ConfigCrestConfigurationService(IConfigCrestHttpClient configCrestHttpClient, IPolicyRegistry<string> policyRegistry,
        ILogger<ConfigCrestConfigurationService> logger)
    {
        _configCrestHttpClient = configCrestHttpClient;
        _policyRegistry = policyRegistry;
        _logger = logger;
    }

    /// <summary>
    /// Gets serialized config value of T type by configurationKey (configurationSetId in ConfigCrest)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configurationKey"></param>
    /// <returns></returns>
    public async Task<T> GetConfigItemAsync<T>(string configurationKey)
    {
        _logger.LogTrace($"Calling {nameof(GetConfigItemAsync)} and parameters: ConfigurationSetId={configurationKey}.");

        var policy = _policyRegistry
            .Get<IAsyncPolicy<ConfigurationResponse>>(Constants.CachePolicies.GlConfigurationCachePolicy);

        var configurationResponse = await policy.ExecuteAsync((context) =>
            _configCrestHttpClient.GetConfigurationAsync(configurationKey), new Context(configurationKey));

        try
        {
            return configurationResponse.ConfigurationValue.JsonDeserialize<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to deserialize configuration item from JSON for ConfigurationSetId={configurationKey};" +
                             $" ConfigurationValue={configurationResponse.ConfigurationValue}.");

            throw new ConfigurationMissingException($"Unable to deserialize configuration item from JSON for ConfigurationSetId={configurationKey}");
        }
    }
}
