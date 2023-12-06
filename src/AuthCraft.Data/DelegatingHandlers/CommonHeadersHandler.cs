using AuthCraft.Common;
using Microsoft.AspNetCore.Http;
using AuthCraft.Common.Extensions;

namespace AuthCraft.Data.DelegatingHandlers;

public class CommonHeadersHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommonHeadersHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext.GetIpFromRequest();
        var correlationId = _httpContextAccessor.HttpContext.Response.Headers[Constants.HeaderKeys.AuthCraftCorrelationIdHeader]
            .FirstOrDefault();

        request.Headers.Add(Constants.HeaderKeys.XForwardedFor, ipAddress);
        request.Headers.Add(Constants.HeaderKeys.AuthCraftCorrelationIdHeader, correlationId);

        return await base.SendAsync(request, cancellationToken);
    }
}
