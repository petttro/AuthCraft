using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AuthCraft.Api.Logging;

namespace AuthCraft.Api.Middleware;

public class LogResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public LogResponseMiddleware(RequestDelegate next, ILogger<LogResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        using (var buffer = new MemoryStream())
        {
            var stream = context.Response.Body;
            context.Response.Body = buffer;

            var timer = Stopwatch.StartNew();

            await _next.Invoke(context);

            timer.Stop();

            buffer.Position = 0;

            using (var bufferReader = new StreamReader(buffer))
            {
                var body = await bufferReader.ReadToEndAsync();
                buffer.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    await buffer.CopyToAsync(stream);
                    context.Response.Body = stream;
                }

                _logger.LogResponse(context, body, timer.ElapsedMilliseconds);
            }
        }
    }
}
