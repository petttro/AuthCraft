using System.Threading.Tasks;
using AuthCraft.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace AuthCraft.Api.Test.Middleware.Headers
{
    public class CustomHeadersMiddlewareTests
    {
        [Fact]
        public async Task CustomHeadersMiddleware_Invoke_Success()
        {
            RequestDelegate next = context => Task.CompletedTask;

            var middleware = new CustomHeadersMiddleware(next);

            var responseHeaders = new HeaderDictionary();
            var responseMock = new Mock<HttpResponse>();
            responseMock.SetupGet(response => response.Headers).Returns(responseHeaders);

            var requestMock = new Mock<HttpRequest>();

            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(response => response.Response).Returns(responseMock.Object);
            contextMock.SetupGet(request => request.Request).Returns(requestMock.Object);
            contextMock.SetupProperty(response => response.TraceIdentifier, "SYSTEM");

            await middleware.Invoke(contextMock.Object);

            Assert.NotNull(responseHeaders);
            Assert.Equal("SYSTEM", responseHeaders["x-authcraft-correlationid"]);
        }
    }
}
