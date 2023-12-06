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

public class ConfigCrestAuthHttpClientTests : MockStrictBehaviorTest
{
    private readonly IConfigurationRoot _configuration;
    private readonly ConfigCrestAuthHttpClient _configCrestAuthHttpClient;
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _magicHttpClient;

    public ConfigCrestAuthHttpClientTests()
    {
        _configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        _configuration["CONFIGCREST_URL"] = "https://some.url.com";

        _handlerMock = _mockRepository.Create<HttpMessageHandler>();
        _magicHttpClient = new (_handlerMock.Object);

        _configCrestAuthHttpClient = new ConfigCrestAuthHttpClient(_magicHttpClient, new NullLogger<ConfigCrestAuthHttpClient>(), _configuration);
    }

    [Fact]
    public async Task GetClientTokenAsync_Success()
    {
        var apiKey = Guid.NewGuid();

        var tokenResponse = new ClientTokenResponse()
        {
            Token = "asdfasdfasdf"
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(tokenResponse.SerializeJsonSafe(), Encoding.UTF8, "application/json")
        };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(
                    r => r.Content.ReadAsStringAsync().Result.Contains(apiKey.ToString()) &&
                         r.RequestUri.ToString().Contains("auth/client")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response)
            .Verifiable();

        await _configCrestAuthHttpClient.GetClientTokenAsync(apiKey);
    }
}
