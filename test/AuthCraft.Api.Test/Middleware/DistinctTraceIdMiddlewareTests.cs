using System.Threading.Tasks;
using AuthCraft.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AuthCraft.Api.Test.Middleware
{
    public class DistinctTraceIdMiddlewareTests
    {
        [Fact]
        public async Task DistinctTraceIdMiddleware_Success()
        {
            RequestDelegate next = context => Task.CompletedTask;

            var middleware = new DistinctTraceIdMiddleware(next);

            Assert.NotNull(middleware);

            var httpContext = new DefaultHttpContext();

            Assert.NotNull(httpContext.TraceIdentifier);
            var trace = httpContext.TraceIdentifier;

            await middleware.Invoke(httpContext);

            Assert.NotEqual(httpContext.TraceIdentifier, trace);
        }
    }
}
