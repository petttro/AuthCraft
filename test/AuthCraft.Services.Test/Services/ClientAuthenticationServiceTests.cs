using System;
using System.Threading.Tasks;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.Exceptions;
using AuthCraft.Common.ServiceInterfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace AuthCraft.Services.Tests.Services;

public class ClientAuthenticationServiceTest : MockStrictBehaviorTest
{
    private readonly Mock<ISecurityTokenService> _securityTokenServiceMock;
    private readonly ClientAuthenticationService _clientAuthenticationService;
    private readonly Mock<IConfigCrestConfigurationService> _configCrestConfigurationServiceMock;

    public ClientAuthenticationServiceTest()
    {
        _securityTokenServiceMock = _mockRepository.Create<ISecurityTokenService>();
        _configCrestConfigurationServiceMock = _mockRepository.Create<IConfigCrestConfigurationService>();

        _clientAuthenticationService = new ClientAuthenticationService(
            new NullLogger<ClientAuthenticationService>(),
            _configCrestConfigurationServiceMock.Object,
            _securityTokenServiceMock.Object);
    }

    [Fact]
    public async Task ClientAuthenticationService_AuthenticateClient_ThrowsConfigurationMissingException()
    {
        var key = Guid.NewGuid();

        _configCrestConfigurationServiceMock
            .Setup(gl => gl.GetConfigItemAsync<ClientList>("clients"))
            .ReturnsAsync(new ClientList());

        // Act
        await Assert.ThrowsAsync<ConfigurationMissingException>(() => _clientAuthenticationService.AuthenticateAsync(key));
    }

    [Fact]
    public async Task ClientAuthenticationService_AuthenticateClient_ThrowsUnauthorizedException()
    {
        var key = Guid.NewGuid();
        var clients = new ClientList { DynamoDbConfigurationMock.GetConfigurationClient() };

        _configCrestConfigurationServiceMock
            .Setup(gl => gl.GetConfigItemAsync<ClientList>("clients"))
            .ReturnsAsync(clients);

        // Act
        await Assert.ThrowsAsync<UnauthorizedException>(() => _clientAuthenticationService.AuthenticateAsync(key));
    }

    [Fact]
    public async Task ClientAuthenticationService_AuthenticateClient_Success()
    {
        var key = DynamoDbConfigurationMock.GetConfigurationClient().Key;
        var clients = new ClientList { DynamoDbConfigurationMock.GetConfigurationClient() };

        _configCrestConfigurationServiceMock
            .Setup(gl => gl.GetConfigItemAsync<ClientList>("clients"))
            .ReturnsAsync(clients);

        var token = new CreateTokenOutput { Token = "Token" };
        _securityTokenServiceMock
            .Setup(u => u.CreateAsync(It.IsAny<ClientList.Client>()))
            .ReturnsAsync(token);

        // Act
        var output = await _clientAuthenticationService.AuthenticateAsync(key);

        Assert.NotNull(output);
        Assert.Equal("Token", output.Token);
    }
}
