using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace AuthCraft.Common.Extensions;

public static class HttpContextExtentions
{
    //http://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
    public static string GetIpFromRequest(this HttpContext httpCxt)
    {
        if (httpCxt == null)
        {
            return null;
        }

        var ip = httpCxt.Connection?.RemoteIpAddress?.ToString();
        if (ip == null)
        {
            ip = httpCxt.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
        }

        StringValues values = new StringValues();
        if (ip == null)
        {
            httpCxt.Request?.Headers?.TryGetValue(Constants.HeaderKeys.XForwardedFor, out values);
            ip = values.FirstOrDefault();
        }

        if (ip == null)
        {
            httpCxt.Request?.Headers?.TryGetValue(Constants.HeaderKeys.RemoteAddr, out values);
            ip = values.FirstOrDefault();
        }

        return ip;
    }

    public static string GetIp(this HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var remoteIpAddress = httpContext.Connection?.RemoteIpAddress?.MapToIPv4(); // TODO: IPv6?

        var headers = httpContext.Request?.Headers;
        string ip = null;

        // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For
        if (headers != null)
        {
            ip = GetHeaderValue(headers, "X-Forwarded-For");
        }

        if (string.IsNullOrWhiteSpace(ip) && remoteIpAddress != null)
        {
            ip = remoteIpAddress.ToString();
        }

        if (string.IsNullOrWhiteSpace(ip) && headers != null)
        {
            ip = GetHeaderValue(headers, "REMOTE_ADDR");
        }

        return ip;
    }

    private static string GetHeaderValue(IHeaderDictionary headers, string headerName)
    {
        var values = headers[headerName];
        return values == StringValues.Empty ? null : values.First().Split(',').FirstOrDefault();
    }
}
