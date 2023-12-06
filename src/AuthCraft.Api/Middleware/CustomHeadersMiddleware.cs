using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AuthCraft.Api.Middleware;

public class CustomHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public CustomHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers["x-authcraft-correlationid"] = context.TraceIdentifier;
        await _next(context);
    }
}
