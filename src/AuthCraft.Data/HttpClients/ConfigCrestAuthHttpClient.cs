using System.Text;
using AuthCraft.Common;
using AuthCraft.Common.Exceptions;
using AuthCraft.Data.Dto.ConfigCrest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuthCraft.Common.Extensions;
using AuthCraft.Data.Extensions;

namespace AuthCraft.Data.HttpClients;

public class ConfigCrestAuthHttpClient : IConfigCrestAuthHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConfigCrestAuthHttpClient> _logger;

    public ConfigCrestAuthHttpClient(HttpClient httpClient, ILogger<ConfigCrestAuthHttpClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri($"{configuration[Constants.EnvironmentVariables.ConfigCrestUrl]}/api/v1/");
        _logger = logger;
    }

    /// <summary>
    /// Makes the actual call to ConfigCrest to get a client token
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetClientTokenAsync(Guid apiKey)
    {
        _logger.LogTrace($"Calling {nameof(ConfigCrestAuthHttpClient)} with parameters ApiKey={apiKey}");

        var requestUrl = "auth/client";
        var clientTokenRequest = new ClientTokenRequest { Key = apiKey };
        var content = new StringContent(clientTokenRequest.JsonSerialize(), Encoding.UTF8, Constants.ContentType.ApplicationJson);
        var httpResponseMessage = await _httpClient.PostAsync(requestUrl, content);
        var response = await httpResponseMessage.Deserialize<ClientTokenResponse>();

        if (response.Success)
        {
            return response.Content.Token;
        }

        throw new InternalSystemException($"Unable to get ConfigCrest Client Token. ErrorDetails={response.ErrorDetails}");
    }
}
