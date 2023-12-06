using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace AuthCraft.Common.Extensions;

public static class HttpHeadersExtensions
{
    private const int MaxHeaderLength = 10000;

    public static string GetHeaderTraceString(this HttpResponseHeaders headers)
    {
        return GetHeaderTraceString(headers as HttpHeaders);
    }

    public static string GetHeaderTraceString(this HttpHeaders headers)
    {
        if (headers == null)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var header in headers)
        {
            var headerValue = header.Value.Any() ? string.Join(",", header.Value) : string.Empty;
            builder.AppendLine($"{header.Key}='{headerValue.Limit(MaxHeaderLength)}'");
        }

        return builder.ToString();
    }
}
