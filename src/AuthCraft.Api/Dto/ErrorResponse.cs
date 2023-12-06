using System.Collections.Generic;
using AuthCraft.Common.Exceptions;

namespace AuthCraft.Api.Dto;

public class ErrorResponse
{
    public CustomErrorCode Code { get; set; }

    public string Message { get; set; }

    public string SplunkLink { get; set; }

    public IDictionary<string, object> InputErrors { get; set; }
}
