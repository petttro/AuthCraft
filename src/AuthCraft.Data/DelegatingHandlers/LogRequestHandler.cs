using System.Diagnostics;
using Microsoft.Extensions.Logging;

using AuthCraft.Common.Extensions;

namespace AuthCraft.Data.DelegatingHandlers;

public class LogRequestHandler : DelegatingHandler
{
    private readonly ILogger<LogRequestHandler> _logger;

    public LogRequestHandler(ILogger<LogRequestHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation($"Starting WebRequest to Verb={request.Method.Method}, Url={request.RequestUri?.AbsoluteUri}");

        var response = await base.SendAsync(request, cancellationToken);

        var requestBody = string.Empty;
        if (request.Content != null)
        {
            requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        var requestUrl = response.RequestMessage?.RequestUri;
        var requestVerb = response.RequestMessage?.Method.Method;
        var requestHeaders = response.RequestMessage?.Headers.GetHeaderTraceString();

        IEnumerable<string> acceptEncoding = null;
        response.RequestMessage?.Headers.TryGetValues("Accept-Encoding", out acceptEncoding);
        var isBinaryResponse = acceptEncoding?.Contains("gzip") ?? false;

        var responseBody = "[ZIPPED BINARY CONTENT]";
        if (!isBinaryResponse)
        {
            responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        }

        var duration = sw.ElapsedMilliseconds;

        var responseStatus = (int)response.StatusCode;
        var responseReason = response.ReasonPhrase;
        var responseHeaders = response.Headers.GetHeaderTraceString();

        var logTemplate = $"WebRequest: Url={requestUrl}, Verb={requestVerb}, RequestHeaders=[{requestHeaders}], WebRequestBody={requestBody}, " +
                          $"WebResponse: Duration={duration}, Status={responseStatus}, Reason={responseReason}, ResponseHeaders=[{responseHeaders}], " +
                          $"WebResponseBody={responseBody}";

        _logger.LogInformation(logTemplate);

        return response;
    }
}
