using System;
using Xunit;

namespace AuthCraft.Api.Test.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void UnauthorizedAccessException_Constructor_Success()
    {
        var content = new UnauthorizedAccessException();
        Assert.NotNull(content);

        var errorMessage = "Error Message";
        content = new UnauthorizedAccessException(errorMessage);
        Assert.NotNull(content);
        Assert.Equal(content.Message, errorMessage);

        var exception = new Exception();
        content = new UnauthorizedAccessException(errorMessage, exception);
        Assert.NotNull(content);
        Assert.Equal(content.Message, errorMessage);
        Assert.Equal(content.InnerException, exception);
    }
}
