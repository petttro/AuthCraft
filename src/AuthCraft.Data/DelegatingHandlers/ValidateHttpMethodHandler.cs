namespace AuthCraft.Data.DelegatingHandlers;

public class ValidateHttpMethodHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var method = request.Method;

        if (method != HttpMethod.Get &&
            method != HttpMethod.Head &&
            method != HttpMethod.Post &&
            method != HttpMethod.Put &&
            method != HttpMethod.Delete)
        {
            throw new Exception("Unsupported HttpRequestMethod: " + method);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
