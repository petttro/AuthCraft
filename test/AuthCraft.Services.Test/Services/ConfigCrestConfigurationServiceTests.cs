using System.Threading.Tasks;
using AuthCraft.App.Services;
using AuthCraft.Common;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.Exceptions;
using AuthCraft.Data.Dto.ConfigCrest;
using AuthCraft.Data.HttpClients;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using AuthCraft.Common.Extensions;
using Polly;
using Polly.Registry;
using Xunit;

namespace AuthCraft.Services.Tests.Services;

public class ConfigCrestConfigurationServiceTests : MockStrictBehaviorTest
{
    private readonly ConfigCrestConfigurationService _configCrestConfigurationService;
    private readonly Mock<IConfigCrestHttpClient> _configCrestHttpClientMock;
    private readonly Mock<IPolicyRegistry<string>> _policyRegistryMock;

    public ConfigCrestConfigurationServiceTests()
    {
        _configCrestHttpClientMock = _mockRepository.Create<IConfigCrestHttpClient>();
        _policyRegistryMock = _mockRepository.Create<IPolicyRegistry<string>>();

        _configCrestConfigurationService = new ConfigCrestConfigurationService(_configCrestHttpClientMock.Object, _policyRegistryMock.Object,
            new NullLogger<ConfigCrestConfigurationService>());
    }

    [Fact]
    public async Task GetConfigItem_Success()
    {
        var configurationKey = "clients";
        var clientList = new ClientList() { new ClientList.Client() { Application = "Tardis" } };
        var configResponse = new ConfigurationResponse()
        {
            ConfigurationValue = clientList.JsonSerialize()
        };

        var mockedPolicy = Policy.NoOpAsync<ConfigurationResponse>();
        _policyRegistryMock
            .Setup(p => p.Get<IAsyncPolicy<ConfigurationResponse>>(Constants.CachePolicies.GlConfigurationCachePolicy))
            .Returns(mockedPolicy);

        _configCrestHttpClientMock
            .Setup(h => h.GetConfigurationAsync(configurationKey))
            .ReturnsAsync(configResponse);

        var result = await _configCrestConfigurationService.GetConfigItemAsync<ClientList>(configurationKey);

        Assert.Equal("Tardis", result[0].Application);
    }

    [Fact]
    public async Task GetConfigItem_UnableDeserializeConfig_ThrowsException()
    {
        var configurationKey = "clients";
        var configResponse = new ConfigurationResponse()
        {
            ConfigurationValue = "some wrong value"
        };

        var mockedPolicy = Policy.NoOpAsync<ConfigurationResponse>();
        _policyRegistryMock
            .Setup(p => p.Get<IAsyncPolicy<ConfigurationResponse>>(Constants.CachePolicies.GlConfigurationCachePolicy))
            .Returns(mockedPolicy);

        _configCrestHttpClientMock
            .Setup(h => h.GetConfigurationAsync(configurationKey))
            .ReturnsAsync(configResponse);

        var ex = await Assert.ThrowsAsync<ConfigurationMissingException>(() => _configCrestConfigurationService.GetConfigItemAsync<ClientList>(configurationKey));
    }
}
