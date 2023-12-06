using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AuthCraft.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthCraft.Api.Test.Middleware.Logging
{
    public class LogResponseMiddlewareTests
    {
        [Fact]
        public async Task LogResponseMiddleware_Invoke_Success()
        {
            RequestDelegate next = context => Task.CompletedTask;

            var logger = new Mock<ILogger<LogResponseMiddleware>>();

            var middleware = new LogResponseMiddleware(next, logger.Object);

            var contents = "Content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            var responseMock = new Mock<HttpResponse>();

            responseMock.SetupProperty(httpRequest => httpRequest.Body, stream);
            responseMock.SetupGet(response => response.Headers).Returns(new HeaderDictionary());
            responseMock.SetupGet(response => response.StatusCode).Returns(200);

            var requestMock = new Mock<HttpRequest>();
            requestMock.SetupProperty(request => request.Body, stream);

            requestMock.SetupGet(request => request.Headers).Returns(new HeaderDictionary());

            requestMock.SetupProperty(request => request.Method, "Get");

            // GetDisplayUrl()
            requestMock.SetupProperty(request => request.Host, new HostString("Host"));
            requestMock.SetupProperty(request => request.PathBase, new PathString("/PathBase"));
            requestMock.SetupProperty(request => request.Path, new PathString("/Path"));
            requestMock.SetupProperty(request => request.QueryString, new QueryString("?key1=val1"));
            requestMock.SetupProperty(request => request.Scheme, "Scheme");

            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(response => response.Response).Returns(responseMock.Object);
            contextMock.SetupGet(response => response.Request).Returns(requestMock.Object);

            var connectionMock = new Mock<ConnectionInfo>();
            connectionMock.SetupGet(response => response.RemoteIpAddress).Returns(IPAddress.Parse("10.12.10.10"));

            contextMock.SetupGet(response => response.Connection).Returns(connectionMock.Object);

            await middleware.Invoke(contextMock.Object);

            Assert.NotNull(stream);
        }
    }
}
