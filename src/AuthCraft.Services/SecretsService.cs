using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using AuthCraft.Common.Config;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.Exceptions;
using AuthCraft.Common.Extensions;
using AuthCraft.Common.Infrastructure.Aws;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthCraft.Services;

public class SecretsService : ISecretsService, IDisposable
{
    private readonly ILogger<SecretsService> _logger;
    private readonly SecretsManagerCache _secretsManagerCache;
    private readonly string _secretId;

    public SecretsService(ILogger<SecretsService> logger, IOptions<EnvironmentConfig> envOptions, IAmazonSecretsManager amazonSecretsManager)
    {
        _logger = logger;
        _secretsManagerCache = new SecretsManagerCache(amazonSecretsManager);
        _secretId = $"tf-authcraft-{envOptions.Value.AwsEnvironment}-{envOptions.Value.AwsRegion}";
    }

    public async Task<SigningInfo> GetSigningKeyAsync()
    {
        var secret = await GetSecretAsync() ?? throw new ConfigurationMissingException($"Secret is null");

        try
        {
            var secretDto = secret.JsonDeserialize<AuthCraftSecrets>()
                            ?? throw new ConfigurationMissingException("Secret is empty");

            var errors = new List<string>();

            if (string.IsNullOrEmpty(secretDto.Audience))
            {
                errors.Add("\"auth:Audience\" property");
            }

            if (string.IsNullOrEmpty(secretDto.Issuer))
            {
                errors.Add("\"auth:Issuer\" property");
            }

            if (string.IsNullOrEmpty(secretDto.Private))
            {
                errors.Add("\"Private\" property");
            }

            if (errors.Any())
            {
                throw new ConfigurationMissingException($"Secret does not contain {string.Join(',', errors)}");
            }

            return new SigningInfo
            {
                Audience = secretDto.Audience,
                Issuer = secretDto.Issuer,
                Private = secretDto.Private
            };
        }
        catch (JsonException je)
        {
            _logger.LogError(je, "Unexpected JSON deserialization error");
            throw new ConfigurationMissingException($"Secret contains invalid JSON");
        }
    }

    public async Task<string> GetApiKeyAsync()
    {
        var secret = await GetSecretAsync() ?? throw new ConfigurationMissingException($"Secret is null");

        try
        {
            var secretDto = secret.JsonDeserialize<AuthCraftSecrets>()
                            ?? throw new ConfigurationMissingException("Secret is empty");

            if (string.IsNullOrEmpty(secretDto.ConfigCrestApiKey))
            {
                throw new ConfigurationMissingException("Secret does not contain a \"auth:ConfigCrestApiKey\" property");
            }

            return secretDto.ConfigCrestApiKey;
        }
        catch (JsonException je)
        {
            _logger.LogError(je, "Unexpected JSON deserialization error");
            throw new ConfigurationMissingException($"Secret contains invalid JSON");
        }
    }

    public void Dispose()
    {
        _secretsManagerCache.Dispose();
    }

    protected virtual async Task<string> GetSecretAsync()
    {
        try
        {
            return await _secretsManagerCache.GetSecretString(_secretId);
        }
        catch (AmazonServiceException ase)
        {
            _logger.LogError(ase, $"Error with obtaining secret {_secretId}");
            throw new ConfigurationMissingException($"Failed to retrieve secret {_secretId}");
        }
    }
}
