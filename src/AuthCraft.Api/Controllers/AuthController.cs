using System.Net;
using System.Threading.Tasks;
using AuthCraft.Api.Dto;
using AuthCraft.Common.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthCraft.Api.Controllers;

[Route("api/")]
public class AuthController : BaseController
{
    private readonly IClientAuthenticationService _clientAuthenticationService;

    public AuthController(IClientAuthenticationService clientAuthenticationService)
    {
        _clientAuthenticationService = clientAuthenticationService;
    }

    /// <summary>
    /// Get a security token, passing which will satisfy Allow:Clients:Any policy
    /// </summary>
    /// <param name="request">Client authentication data</param>
    /// <returns>Auth token and it's expiration time</returns>
    [ProducesResponseType(typeof(ClientAuthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [HttpPost("[controller]/client")]
    public async Task<IActionResult> AuthenticateClient([FromBody] ClientAuthRequest request)
    {
        var output = await _clientAuthenticationService.AuthenticateAsync(request.Key.Value);
        return Ok(new ClientAuthResponse
        {
            Application = output.Application,
            Token = output.Token,
            ExpiresAt = output.ExpiresAt
        });
    }
}
