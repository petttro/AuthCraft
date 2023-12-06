using System.Net.Http.Headers;
using AuthCraft.Common.Extensions;
using Xunit;

namespace AuthCraft.Common.Test.Extensions;

public class HttpHeadersExtensionsTests
{
    [Fact]
    public void GetHeaderTraceString_RequestHeaders_EmptyHeaders()
    {
        var httpHeaders = new HttpClient().DefaultRequestHeaders;
        var headersTraceString = httpHeaders.GetHeaderTraceString();

        Assert.Equal(string.Empty, headersTraceString);
    }

    [Fact]
    public void GetHeaderTraceString_RequestHeaders_NullHeaders()
    {
        HttpRequestHeaders httpHeaders = null;
        var headersTraceString = httpHeaders.GetHeaderTraceString();

        Assert.Equal(string.Empty, headersTraceString);
    }

    [Fact]
    public void GetHeaderTraceString_RequestHeaders()
    {
        var httpHeaders = new HttpClient().DefaultRequestHeaders;
        httpHeaders.Add("x-forwarded-for", new[] { "10.10.10.10", "20.20.20.20" });
        httpHeaders.Add("Accept", "application/json");
        httpHeaders.Add("x-authcraft-correlationid", "7f54ad45-0d02-49fe-a39a-e6220bb135e8");
        httpHeaders.Add("Authorization", "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJDb25maWd1cmF0aW9ucyI6IlJlYWQiLCJHaG9z");
        httpHeaders.Add("x-b3-traceid", "5ba48903bfaa923743583b0659b0a352");
        httpHeaders.Add("x-b3-spanid", "a4de226d0e599bb5");
        httpHeaders.Add("traceparent", "00-b445bff672476a691175e80b5c98e174-1afbd77633241de9-00");

        var headersTraceString = httpHeaders.GetHeaderTraceString();

        var expectedResult =
            @"x-forwarded-for='10.10.10.10,20.20.20.20'
Accept='application/json'
x-authcraft-correlationid='7f54ad45-0d02-49fe-a39a-e6220bb135e8'
Authorization='bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJDb25maWd1cmF0aW9ucyI6IlJlYWQiLCJHaG9z'
x-b3-traceid='5ba48903bfaa923743583b0659b0a352'
x-b3-spanid='a4de226d0e599bb5'
traceparent='00-b445bff672476a691175e80b5c98e174-1afbd77633241de9-00'
";

        Assert.Equal(expectedResult, headersTraceString);
    }

    [Fact]
    public void GetHeaderTraceString_ResponseHeaders()
    {
        var message = new HttpResponseMessage();
        var httpHeaders = message.Headers;
        httpHeaders.Add("x-forwarded-for", new[] { "10.10.10.10", "20.20.20.20" });
        httpHeaders.Add("Accept", "application/json");
        httpHeaders.Add("x-authcraft-correlationid", "7f54ad45-0d02-49fe-a39a-e6220bb135e8");
        httpHeaders.Add("Authorization", "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJDb25maWd1cmF0aW9ucyI6IlJlYWQiLCJHaG9z");
        httpHeaders.Add("x-b3-traceid", "5ba48903bfaa923743583b0659b0a352");
        httpHeaders.Add("x-b3-spanid", "a4de226d0e599bb5");
        httpHeaders.Add("traceparent", "00-b445bff672476a691175e80b5c98e174-1afbd77633241de9-00");

        var headersTraceString = httpHeaders.GetHeaderTraceString();

        var expectedResult =
            @"x-forwarded-for='10.10.10.10,20.20.20.20'
Accept='application/json'
x-authcraft-correlationid='7f54ad45-0d02-49fe-a39a-e6220bb135e8'
Authorization='bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJDb25maWd1cmF0aW9ucyI6IlJlYWQiLCJHaG9z'
x-b3-traceid='5ba48903bfaa923743583b0659b0a352'
x-b3-spanid='a4de226d0e599bb5'
traceparent='00-b445bff672476a691175e80b5c98e174-1afbd77633241de9-00'
";

        Assert.Equal(expectedResult, headersTraceString);
    }
}
