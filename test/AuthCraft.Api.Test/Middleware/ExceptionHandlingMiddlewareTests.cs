using System;
using System.Net;
using System.Threading.Tasks;
using AuthCraft.Api.Configs;
using AuthCraft.Api.Middleware;
using AuthCraft.Api.Test.Common;
using AuthCraft.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AuthCraft.Api.Test.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public void ExceptionHandlingMiddleware_Constructor_NullSplunkConfigThrowsNullReferenceException()
        {
            RequestDelegate next = context => Task.CompletedTask;

            IOptionsSnapshot<SplunkConfig> splunkConfig = null;
            var env = new Mock<IWebHostEnvironment>().Object;

            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>().Object;
            Assert.Throws<NullReferenceException>(() => new ExceptionHandlingMiddleware(next, logger, splunkConfig, env));
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_Success()
        {
            RequestDelegate next = context => Task.CompletedTask;

            var splunkConfig = new OptionsSnapshotWrapper<SplunkConfig>(new SplunkConfig());
            var env = new Mock<IWebHostEnvironment>().Object;
            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>().Object;
            var middleware = new ExceptionHandlingMiddleware(next, logger, splunkConfig, env);

            Assert.NotNull(middleware);

            var httpContext = new DefaultHttpContext();

            await middleware.Invoke(httpContext);
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_InvokeThrowsException_Success()
        {
            var errorMessage = "errorMessage";
            RequestDelegate next = context => { throw new Exception(errorMessage); };

            var splunkConfig = new OptionsSnapshotWrapper<SplunkConfig>(new SplunkConfig());
            var env = new Mock<IWebHostEnvironment>().Object;

            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>().Object;
            var middleware = new ExceptionHandlingMiddleware(next, logger, splunkConfig, env);

            Assert.NotNull(middleware);

            var httpContext = new DefaultHttpContext();

            await middleware.Invoke(httpContext);

            Assert.NotNull(httpContext);
            Assert.NotNull(httpContext.Response);
            Assert.Equal("application/json", httpContext.Response.ContentType);
            Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_InvokeThrowsBadUserInputException_Success()
        {
            var errorMessage = "errorMessage";
            RequestDelegate next = context => { throw new BadUserInputException(errorMessage); };

            var splunkConfig = new OptionsSnapshotWrapper<SplunkConfig>(new SplunkConfig());
            var env = new Mock<IWebHostEnvironment>().Object;

            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>().Object;
            var middleware = new ExceptionHandlingMiddleware(next, logger, splunkConfig, env);

            Assert.NotNull(middleware);

            var httpContext = new DefaultHttpContext();

            await middleware.Invoke(httpContext);

            Assert.NotNull(httpContext);
            Assert.NotNull(httpContext.Response);
            Assert.Equal("application/json", httpContext.Response.ContentType);
            Assert.Equal((int)HttpStatusCode.BadRequest, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_InvokeThrowsNotFoundException_Success()
        {
            var errorMessage = "errorMessage";
            RequestDelegate next = context => { throw new NotFoundException(errorMessage); };

            var splunkConfig = new OptionsSnapshotWrapper<SplunkConfig>(new SplunkConfig());
            var env = new Mock<IWebHostEnvironment>().Object;

            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>().Object;
            var middleware = new ExceptionHandlingMiddleware(next, logger, splunkConfig, env);

            Assert.NotNull(middleware);

            var httpContext = new DefaultHttpContext();

            await middleware.Invoke(httpContext);

            Assert.NotNull(httpContext);
            Assert.NotNull(httpContext.Response);
            Assert.Equal("application/json", httpContext.Response.ContentType);
            Assert.Equal((int)HttpStatusCode.NotFound, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_InvokeThrowsForbiddenException_Success()
        {
            var errorMessage = "errorMessage";
            RequestDelegate next = context => { throw new ForbiddenException(errorMessage); };

            var splunkConfig = new OptionsSnapshotWrapper<SplunkConfig>(new SplunkConfig());
            var env = new Mock<IWebHostEnvironment>().Object;

            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>().Object;
            var middleware = new ExceptionHandlingMiddleware(next, logger, splunkConfig, env);

            Assert.NotNull(middleware);

            var httpContext = new DefaultHttpContext();

            await middleware.Invoke(httpContext);

            Assert.NotNull(httpContext);
            Assert.NotNull(httpContext.Response);
            Assert.Equal("application/json", httpContext.Response.ContentType);
            Assert.Equal((int)HttpStatusCode.Forbidden, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_InvokeThrowsUnauthorizedException_Success()
        {
            var errorMessage = "errorMessage";
            RequestDelegate next = context => { throw new UnauthorizedException(errorMessage); };

            var splunkConfig = new OptionsSnapshotWrapper<SplunkConfig>(new SplunkConfig());
            var env = new Mock<IWebHostEnvironment>().Object;

            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>().Object;
            var middleware = new ExceptionHandlingMiddleware(next, logger, splunkConfig, env);

            Assert.NotNull(middleware);

            var httpContext = new DefaultHttpContext();

            await middleware.Invoke(httpContext);

            Assert.NotNull(httpContext);
            Assert.NotNull(httpContext.Response);
            Assert.Equal("application/json", httpContext.Response.ContentType);
            Assert.Equal((int)HttpStatusCode.Unauthorized, httpContext.Response.StatusCode);
        }
    }
}
