using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AuthCraft.Api.Logging;

namespace AuthCraft.Api.Middleware;

public class LogRequestMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public LogRequestMiddleware(RequestDelegate next, ILogger<LogRequestMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;

        using (var stream = new StreamReader(context.Request.Body))
        {
            var requestBodyText = await stream.ReadToEndAsync();
            context.Request.Body.Position = 0;

            _logger.LogRequest(context, requestBodyText);

            await _next(context);
        }
    }
}
