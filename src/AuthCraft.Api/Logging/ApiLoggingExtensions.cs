using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using AuthCraft.Common.Extensions;

namespace AuthCraft.Api.Logging;

public static class ApiLoggingExtensions
{
    private const int MaxHeaderLength = 10000;

    public static void LogRequest(this ILogger logger, HttpContext context, string requestBody)
    {
        var ipAddress = context.GetIp();

        var requestHeadersString = GetHeaderTraceString(context.Request.Headers);
        logger.LogInformation(
            $"ApiRequest, Verb={context.Request.Method}, Url={context.Request.GetDisplayUrl()}, " +
            $"Ip={ipAddress}, RequestHeaders=[{requestHeadersString}], Body='{requestBody}'");
    }

    public static void LogResponse(this ILogger logger, HttpContext context, string responseBody, long durationMilliseconds)
    {
        var ipAddress = context.GetIp();

        var requestHeadersString = GetHeaderTraceString(context.Request.Headers);
        var responseHeadersString = GetHeaderTraceString(context.Response.Headers);
        var durationMillisString = durationMilliseconds == long.MinValue ? string.Empty : $"DurationMillis={durationMilliseconds}";

        logger.LogInformation(
            $"ApiResponse, Verb={context.Request.Method}, Url={context.Request.GetDisplayUrl()}, Status={context.Response.StatusCode}, " +
            $"Ip={ipAddress}, {durationMillisString}, RequestHeaders=[{requestHeadersString}], ResponseHeaders=[{responseHeadersString}], Body='{responseBody}'");
    }

    private static string GetHeaderTraceString(IHeaderDictionary headers)
    {
        if (headers == null || !headers.Any())
        {
            return null;
        }

        var joinedHeaders = string.Join(", ", headers.Select(header => $"{header.Key}='{string.Join(",", header.Value.Any() ? header.Value.ToString() : string.Empty)}'"));

        // Limit the string to 10000 symbols
        return joinedHeaders.Limit(MaxHeaderLength);
    }
}
