using System.Net.Http.Headers;
using AuthCraft.Common;
using AuthCraft.Common.Infrastructure.Aws;
using AuthCraft.Data.HttpClients;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;

namespace AuthCraft.Data.DelegatingHandlers;

public class ConfigCrestAuthenticationHandler : DelegatingHandler
{
    private readonly IConfigCrestAuthHttpClient _httpClient;
    private readonly IPolicyRegistry<string> _policyRegistry;
    private readonly ILogger<ConfigCrestAuthenticationHandler> _logger;
    private readonly ISecretsService _secretsService;

    public ConfigCrestAuthenticationHandler(
        IConfigCrestAuthHttpClient httpClient,
        ISecretsService secretsService,
        IPolicyRegistry<string> policyRegistry,
        ILogger<ConfigCrestAuthenticationHandler> logger)
    {
        _httpClient = httpClient;
        _policyRegistry = policyRegistry;
        _logger = logger;
        _secretsService = secretsService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogTrace($"Calling {nameof(ConfigCrestAuthenticationHandler)} Url={request.RequestUri?.AbsoluteUri}");

        var apiKey = Guid.Parse(await _secretsService.GetApiKeyAsync());
        var policy = _policyRegistry.Get<IAsyncPolicy<string>>(Constants.CachePolicies.GlClientTokenCachePolicy);
        var clientToken = await policy.ExecuteAsync(context =>
            _httpClient.GetClientTokenAsync(apiKey), new Context(Constants.CacheKeys.GlClientTokenCacheKey));

        _logger.LogDebug($"Got GL ClientToken={clientToken}");

        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", clientToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
