using System.Net;
using AuthCraft.Api.Attributes;
using AuthCraft.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AuthCraft.Api.Controllers;

[ModelValidator]
[Produces("application/json")]
[ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden)]
public abstract class BaseController : Controller
{
    protected IActionResult StatusCode(HttpStatusCode statusCode, object value = null)
    {
        return StatusCode((int)statusCode, value);
    }
}
