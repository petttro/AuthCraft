using AuthCraft.Api.Health;
using AuthCraft.Common.Infrastructure.Aws;
using AuthCraft.Common.ServiceInterfaces;
using AuthCraft.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCraft.Api;

public static class AddCustomServicesExtensions
{
    /// <summary>
    /// Configure custom self written services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IHealthIndicator, EnvironmentHealthIndicator>()
            .AddSingleton<ISecretsService, SecretsService>()
            .AddTransient<IClientAuthenticationService, ClientAuthenticationService>()
            .AddSingleton<ISecurityTokenService, SecurityTokenService>()
            .AddTransient<IConfigCrestConfigurationService, ConfigCrestConfigurationService>();

        return services;
    }
}
