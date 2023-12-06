using System;
using AuthCraft.App.Services;
using AuthCraft.Common;
using AuthCraft.Data.Dto.ConfigCrest;
using AuthCraft.Data.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Caching;
using Polly.Registry;

namespace AuthCraft.Api;

public static class AddCachePoliciesExtensions
{
    /// <summary>
    /// Configure Polly cache policies and add them into Policy Registry
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCachePolicies(this IServiceCollection services)
    {
        services.AddSingleton<IAsyncCacheProvider, Polly.Caching.Memory.MemoryCacheProvider>();

        var policyRegistry = new PolicyRegistry();

        services.AddSingleton<IPolicyRegistry<string>, PolicyRegistry>((serviceProvider) =>
        {
            // Cache policy for GL Configuration Service
            var configurationServiceLogger = serviceProvider.GetService<ILogger<ConfigCrestConfigurationService>>();
            var configCrestHttpClientLogger = serviceProvider.GetService<ILogger<ConfigCrestCrestHttpClient>>();

            Ttl ConfigurationCacheStrategy(ConfigurationResponse configurationResponse) =>
                new Ttl(TimeSpan.FromSeconds(configurationResponse.CacheDurationSeconds));

            var glConfigurationCachePolicy = Policy.CacheAsync(
                cacheProvider: serviceProvider.GetRequiredService<IAsyncCacheProvider>().AsyncFor<ConfigurationResponse>(),
                ttlStrategy: new ResultTtl<ConfigurationResponse>(ConfigurationCacheStrategy),
                onCacheGet: (context, cacheKey) => { configurationServiceLogger.LogDebug($"Got configuration from cache for CacheKey={cacheKey}"); },
                onCacheMiss: (context, cacheKey) => { configurationServiceLogger.LogDebug($"Miss cache for CacheKey={cacheKey}"); },
                onCachePut: (context, cacheKey) => { configurationServiceLogger.LogDebug($"Put configuration to cache for CacheKey={cacheKey}."); },
                onCacheGetError: (context, cacheKey, exception) =>
                {
                    configurationServiceLogger.LogError($"Error get configuration from cache for CacheKey={cacheKey},  Exception={exception}");
                },
                onCachePutError: (context, cacheKey, exception) =>
                {
                    configurationServiceLogger.LogError($"Error put configuration to cache for CacheKey={cacheKey}, Exception={exception}");
                }
            );

            var glClientTokenCachePolicy = Policy.CacheAsync(
                cacheProvider: serviceProvider.GetRequiredService<IAsyncCacheProvider>().AsyncFor<string>(),
                ttl: Constants.Cache.GlClientTokenTtl,
                onCacheGet: (context, cacheKey) => { configCrestHttpClientLogger.LogDebug($"Got GL ClientToken from cache for CacheKey={cacheKey}"); },
                onCacheMiss: (context, cacheKey) => { configCrestHttpClientLogger.LogDebug($"Miss GL ClientToken in cache for CacheKey={cacheKey}"); },
                onCachePut: (context, cacheKey) => { configCrestHttpClientLogger.LogDebug($"Put GL ClientToken to cache for CacheKey={cacheKey}."); },
                onCacheGetError: (context, cacheKey, exception) =>
                {
                    configCrestHttpClientLogger.LogError($"Error get GL ClientToken from cache for CacheKey={cacheKey},  Exception={exception}");
                },
                onCachePutError: (context, cacheKey, exception) =>
                {
                    configCrestHttpClientLogger.LogError($"Error put GL ClientToken to cache for CacheKey={cacheKey}, Exception={exception}");
                }
            );

            policyRegistry.Add(Constants.CachePolicies.GlConfigurationCachePolicy, glConfigurationCachePolicy);
            policyRegistry.Add(Constants.CachePolicies.GlClientTokenCachePolicy, glClientTokenCachePolicy);

            return policyRegistry;
        });

        return services;
    }
}
