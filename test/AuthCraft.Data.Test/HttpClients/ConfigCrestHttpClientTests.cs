using System.Net;
using System.Text;
using AuthCraft.Data.Dto.ConfigCrest;
using AuthCraft.Data.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using AuthCraft.Common.Extensions;
using Xunit;

namespace AuthCraft.Data.Test.HttpClients;

public class ConfigCrestHttpClientTests : MockStrictBehaviorTest
{
    private readonly IConfigurationRoot _configuration;
    private readonly ConfigCrestCrestHttpClient _configCrestCrestHttpClient;
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _magicHttpClient;

    public ConfigCrestHttpClientTests()
    {
        _configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        _configuration["CONFIGCREST_URL"] = "https://some.url.com";

        _handlerMock = _mockRepository.Create<HttpMessageHandler>();
        _magicHttpClient = new (_handlerMock.Object);

        _configCrestCrestHttpClient = new ConfigCrestCrestHttpClient(_magicHttpClient, new NullLogger<ConfigCrestCrestHttpClient>(), _configuration);
    }

    [Fact]
    public async Task GetConfigurationAsync_Success()
    {
        var configurationSetId = "configKey";

        var configurationResponse = new ConfigurationResponse()
        {
            Application = "Tardis",
            ConfigurationValue = "configValue",
            ConfigurationSetId = configurationSetId,
            CacheDurationSeconds = 12,
            ContentType = "application/json",
            Comments = "Some Comments",
            ConfigurationVersion = 2,
            LastChangedBy = "UnitTest",
            LastUpdateDateTime = DateTime.UtcNow
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(configurationResponse.SerializeJsonSafe(), Encoding.UTF8, "application/json")
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().Contains($"configurations/{configurationSetId}/current")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response)
            .Verifiable();

        var result = await _configCrestCrestHttpClient.GetConfigurationAsync(configurationSetId);

        Assert.Equal(configurationResponse.Application, result.Application);
    }
}
