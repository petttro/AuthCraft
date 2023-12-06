using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AuthCraft.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthCraft.Api.Test.Middleware.Logging;

public class LogRequestMiddlewareTests
{
    [Fact]
    public async Task Invoke_Success()
    {
        RequestDelegate next = context => Task.CompletedTask;

        var logger = new Mock<ILogger<LogRequestMiddleware>>().Object;
        var middleware = new LogRequestMiddleware(next, logger);

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

        await middleware.Invoke(contextMock.Object);

        Assert.NotNull(stream);
    }

    [Fact]
    public async Task Invoke_LongHeaders_Success()
    {
        RequestDelegate next = context => Task.CompletedTask;

        var logger = new Mock<ILogger<LogRequestMiddleware>>().Object;
        var middleware = new LogRequestMiddleware(next, logger);

        var contents = "Content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupProperty(httpRequest => httpRequest.Body, stream);
        requestMock.SetupGet(httpRequest => httpRequest.Headers).Returns(new HeaderDictionary()
        {
            { "Header1", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header2", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header3", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header4", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header5", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header6", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header7", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header8", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header9", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
            { "Header10", "Long long long long long long long long long long long long long long long long long long long long long long long Header1 Value" },
        });

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

        await middleware.Invoke(contextMock.Object);

        Assert.NotNull(stream);
    }
}
