using System;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using AuthCraft.App.Services;
using AuthCraft.Common.Config;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using AuthCraft.Common.Extensions;
using Xunit;

namespace AuthCraft.Services.Tests.Services;

public class SecretsServiceTests : MockStrictBehaviorTest
{
    private readonly Mock<IAmazonSecretsManager> _amazonSecretsManagerMock;
    private readonly Mock<IOptions<EnvironmentConfig>> _envOptionsMock;

    private readonly SecretsServiceMock _secretsService;

    public SecretsServiceTests()
    {
        _amazonSecretsManagerMock = _mockRepository.Create<IAmazonSecretsManager>();
        _envOptionsMock = _mockRepository.Create<IOptions<EnvironmentConfig>>();

        _envOptionsMock
            .Setup(x => x.Value)
            .Returns(new EnvironmentConfig());

        _secretsService = new SecretsServiceMock(new NullLogger<SecretsServiceMock>(), _envOptionsMock.Object, _amazonSecretsManagerMock.Object);
    }

    [Fact]
    public async Task GetSigningKeyAsync_SecretsManagerThrowsException()
    {
        var secretsService = new SecretsServiceMock(new NullLogger<SecretsServiceMock>(), _envOptionsMock.Object, _amazonSecretsManagerMock.Object, true);
        var result = await Assert.ThrowsAsync<ConfigurationMissingException>(() => secretsService.GetSigningKeyAsync());

        Assert.Equal("Failed to retrieve secret", result.Message);
    }

    [Fact]
    public async Task GetSigningKeyAsync_SecretsManagerReturnsEmptyString()
    {
        var secretsService = new SecretsServiceMock(
            new NullLogger<SecretsServiceMock>(), _envOptionsMock.Object, _amazonSecretsManagerMock.Object, secret: string.Empty);
        var result = await Assert.ThrowsAsync<ConfigurationMissingException>(() => secretsService.GetSigningKeyAsync());

        Assert.Equal("Secret is empty", result.Message);
    }

    [Fact]
    public async Task GetSigningKeyAsync_SecretsManagerReturnsIncompleteSecret()
    {
        var secretsService = new SecretsServiceMock(
            new NullLogger<SecretsServiceMock>(), _envOptionsMock.Object, _amazonSecretsManagerMock.Object, secret: new AuthCraftSecrets().JsonSerialize());
        var result = await Assert.ThrowsAsync<ConfigurationMissingException>(() => secretsService.GetSigningKeyAsync());

        Assert.Equal("Secret does not contain \"auth:Audience\" property,\"auth:Issuer\" property,\"Private\" property", result.Message);
    }

    [Fact]
    public async Task GetSigningKeyAsync_Succeed()
    {
        var result = await _secretsService.GetSigningKeyAsync();

        Assert.NotNull(result);
        Assert.Equal("authcraft", result.Audience);
        Assert.Equal("moq", result.Issuer);
        Assert.Equal("privateKey", result.Private);
    }

    [Fact]
    public async Task GetApiKeyAsync_SecretsManagerReturnsNull()
    {
        var secretsService = new SecretsServiceMock(new NullLogger<SecretsServiceMock>(), _envOptionsMock.Object, _amazonSecretsManagerMock.Object, returnsNullSecret: true);
        var result = await Assert.ThrowsAsync<ConfigurationMissingException>(() => secretsService.GetSigningKeyAsync());

        Assert.Equal("Secret is null", result.Message);
    }

    [Fact]
    public async Task GetApiKeyAsync_SecretsManagerReturnsIncompleteSecret()
    {
        var secretsService = new SecretsServiceMock(
            new NullLogger<SecretsServiceMock>(), _envOptionsMock.Object, _amazonSecretsManagerMock.Object, secret: new AuthCraftSecrets().JsonSerialize());
        var result = await Assert.ThrowsAsync<ConfigurationMissingException>(() => secretsService.GetApiKeyAsync());

        Assert.Equal("Secret does not contain a \"auth:ConfigCrestApiKey\" property", result.Message);
    }

    [Fact]
    public async Task GetApiKeyAsync_Succeed()
    {
        var result = await _secretsService.GetApiKeyAsync();

        Assert.NotNull(result);
        Assert.Equal("apikey", result);
    }

    public class SecretsServiceMock : SecretsService
    {
        private readonly bool _throwsExpection;
        private readonly bool _returnsNullSecret;
        private readonly string _secret;

        public SecretsServiceMock(
            ILogger<SecretsService> logger,
            IOptions<EnvironmentConfig> envOptions,
            IAmazonSecretsManager amazonSecretsManager,
            bool throwsException = false,
            bool returnsNullSecret = false,
            string secret = null)
            : base(logger, envOptions, amazonSecretsManager)
        {
            _throwsExpection = throwsException;
            _returnsNullSecret = returnsNullSecret;
            _secret = secret;
        }

        protected override async Task<string> GetSecretAsync()
        {
            if (_returnsNullSecret)
            {
                return null;
            }

            if (_secret != null)
            {
                return _secret;
            }

            if (!_throwsExpection)
            {
                return new AuthCraftSecrets
                {
                    Audience = "authcraft",
                    ConfigCrestApiKey = "apikey",
                    Issuer = "moq",
                    Private = "privateKey"
                }.JsonSerialize();
            }

            throw new ConfigurationMissingException($"Failed to retrieve secret");
        }
    }
}
