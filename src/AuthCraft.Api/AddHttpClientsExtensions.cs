using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthCraft.Common;
using AuthCraft.Data.DelegatingHandlers;
using AuthCraft.Data.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace AuthCraft.Api;

public static class AddHttpClientsExtensions
{
    /// <summary>
    /// Http clients configuration and request policies definition
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddTransient<CommonHeadersHandler>();
        services.AddTransient<ValidateHttpMethodHandler>();
        services.AddTransient<LogRequestHandler>();
        services.AddTransient<ConfigCrestAuthenticationHandler>();

        // Retry policy handler
        IAsyncPolicy<HttpResponseMessage> WaitAndRetryPolicy(IServiceProvider serviceProvider, HttpRequestMessage request)
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                .WaitAndRetryAsync(
                    sleepDurations: new[]
                    {
                        TimeSpan.FromMilliseconds(100),
                        TimeSpan.FromMilliseconds(300),
                        TimeSpan.FromMilliseconds(500)
                    },
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        serviceProvider.GetService<ILogger<HttpClient>>()
                            .LogWarning($"Retrying Url={request.RequestUri.AbsoluteUri} {timespan.TotalMilliseconds} ms, then making retry {retryAttempt}.");
                    });
        }

        // Timeout policy handler
        IAsyncPolicy<HttpResponseMessage> TimeoutPolicy(IServiceProvider serviceProvider, HttpRequestMessage request)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                timeout: TimeSpan.FromMilliseconds(Constants.HttpValues.DefaultHttpRequestTimeoutMilliseconds),
                timeoutStrategy: TimeoutStrategy.Optimistic,
                onTimeoutAsync: (context, duration, task, exception) =>
                {
                    serviceProvider.GetService<ILogger<HttpClient>>()
                        .LogError($"Http request Timeout. Url={request.RequestUri.AbsoluteUri}, Timeout={duration} ms, Exception={exception}");

                    return Task.CompletedTask;
                });
        }

        // ConfigCrest AUTH Http Client
        services
            .AddHttpClient<IConfigCrestAuthHttpClient, ConfigCrestAuthHttpClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType.ApplicationJson));
            })

            // Delegating Handlers
            .AddHttpMessageHandler<CommonHeadersHandler>()
            .AddHttpMessageHandler<ValidateHttpMethodHandler>()
            .AddHttpMessageHandler<LogRequestHandler>()

            // Policies
            .AddPolicyHandler(WaitAndRetryPolicy)
            .AddPolicyHandler(TimeoutPolicy); // We place the timeoutPolicy inside the retryPolicy, to make it time out each try.

        // ConfigCrest Http Client
        services
            .AddHttpClient<IConfigCrestHttpClient, ConfigCrestCrestHttpClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentType.ApplicationJson));
            })

            // Delegating Handlers
            .AddHttpMessageHandler<CommonHeadersHandler>()
            .AddHttpMessageHandler<ValidateHttpMethodHandler>()
            .AddHttpMessageHandler<ConfigCrestAuthenticationHandler>()
            .AddHttpMessageHandler<LogRequestHandler>()

            // Policies
            .AddPolicyHandler(WaitAndRetryPolicy)
            .AddPolicyHandler(TimeoutPolicy); // We place the timeoutPolicy inside the retryPolicy, to make it time out each try.

        return services;
    }
}
