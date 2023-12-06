using System;
using System.Threading.Tasks;
using AuthCraft.Api.Controllers;
using AuthCraft.Api.Dto;
using AuthCraft.Common.DomainObjects;
using AuthCraft.Common.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthCraft.Api.Test.Controllers;

public class AuthControllerTests : MockStrictBehaviorTest
{
    private readonly Mock<IClientAuthenticationService> _clientAuthenticationServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _clientAuthenticationServiceMock = _mockRepository.Create<IClientAuthenticationService>();
        _authController = new AuthController(_clientAuthenticationServiceMock.Object);
    }

    [Fact]
    public async Task AuthController_AuthenticateClient_Success()
    {
        // Arrange
        var request = new ClientAuthRequest()
        {
            Key = Guid.NewGuid(),
        };

        var response = new ClientAuthenticationOutput
        {
            Token = "token",
            ExpiresAt = DateTime.UtcNow,
            Application = "SomeApp"
        };

        _authController.ControllerContext.HttpContext = new DefaultHttpContext();

        _clientAuthenticationServiceMock
            .Setup(service => service.AuthenticateAsync(It.IsAny<Guid>()))
            .ReturnsAsync(response);

        // Act
        var result = await _authController.AuthenticateClient(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<ClientAuthResponse>(okResult.Value);

        Assert.NotNull(returnValue);
        Assert.Equal(returnValue.Token, response.Token);
        Assert.Equal(returnValue.ExpiresAt, response.ExpiresAt);
    }
}
