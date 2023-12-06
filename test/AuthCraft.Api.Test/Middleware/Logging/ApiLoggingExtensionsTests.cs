using System.IO;
using System.Net;
using System.Text;
using AuthCraft.Api.Logging;
using AuthCraft.Api.Test.Common;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace AuthCraft.Api.Test.Middleware.Logging
{
    public class ApiLoggingExtensionsTests
    {
        [Fact]
        public void ApiLoggingExtensions_LogRequest_Success()
        {
            var logger = TestLoggerFactory.CreateLogger("Logger");
            string requestBody = "request";

            var contents = "Content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));

            var requestMock = new Mock<HttpRequest>();
            requestMock.SetupProperty(httpRequest => httpRequest.Body, stream);
            requestMock.SetupGet(httpRequest => httpRequest.Headers).Returns(new HeaderDictionary());

            requestMock.SetupProperty(httpRequest => httpRequest.Method, "Get");

            // GetDisplayUrl()
            requestMock.SetupProperty(httpRequest => httpRequest.Host, new HostString("Host"));
            requestMock.SetupProperty(httpRequest => httpRequest.PathBase, new PathString("/PathBase"));
            requestMock.SetupProperty(httpRequest => httpRequest.Path, new PathString("/Path"));
            requestMock.SetupProperty(httpRequest => httpRequest.QueryString, new QueryString("?key1=val1"));
            requestMock.SetupProperty(httpRequest => httpRequest.Scheme, "Scheme");

            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(response => response.Request).Returns(requestMock.Object);

            var connectionMock = new Mock<ConnectionInfo>();
            connectionMock.SetupGet(response => response.RemoteIpAddress).Returns(IPAddress.Parse("10.12.10.10"));
            contextMock.SetupGet(response => response.Connection).Returns(connectionMock.Object);

            ApiLoggingExtensions.LogRequest(logger, contextMock.Object, requestBody);
        }

        [Fact]
        public void ApiLoggingExtensions_LogResponse_Success()
        {
            var logger = TestLoggerFactory.CreateLogger("Logger");
            string responseBody = "response";

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

            ApiLoggingExtensions.LogResponse(logger, contextMock.Object, responseBody, 0);
        }
    }
}
