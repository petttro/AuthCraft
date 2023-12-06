using AuthCraft.Common;
using AuthCraft.Common.Exceptions;
using AuthCraft.Data.Dto.ConfigCrest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuthCraft.Data.Extensions;

namespace AuthCraft.Data.HttpClients;

public class ConfigCrestCrestHttpClient : IConfigCrestHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConfigCrestCrestHttpClient> _logger;

    public ConfigCrestCrestHttpClient(HttpClient httpClient, ILogger<ConfigCrestCrestHttpClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri($"{configuration[Constants.EnvironmentVariables.ConfigCrestUrl]}/api/v1/");
        _logger = logger;
    }

    public async Task<ConfigurationResponse> GetConfigurationAsync(string configurationSetId)
    {
        _logger.LogTrace($"Calling {nameof(GetConfigurationAsync)} with parameters ConfigurationSetId={configurationSetId}");

        var requestUrl = $"configurations/{configurationSetId}/current";

        var httpResponseMessage = await _httpClient.GetAsync(requestUrl);

        var response = await httpResponseMessage.Deserialize<ConfigurationResponse>();

        if (response.Success)
        {
            return response.Content;
        }

        throw new InternalSystemException($"Unable to get configuration ConfigurationSetId={configurationSetId} from ConfigCrest.")
        {
            CustomOverrideMessage = "Configuration load failed or missing."
        };
    }
}
