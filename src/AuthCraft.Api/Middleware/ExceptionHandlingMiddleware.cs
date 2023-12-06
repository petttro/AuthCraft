using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AuthCraft.Api.Configs;
using AuthCraft.Api.Dto;
using AuthCraft.Common;
using AuthCraft.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AuthCraft.Common.Extensions;

namespace AuthCraft.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _env;
    private readonly SplunkConfig _splunkConfig;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(
        RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IOptionsSnapshot<SplunkConfig> splunkConfig, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
        _splunkConfig = splunkConfig.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseInputException ex)
        {
            var httpErrorCode = HttpStatusCode.BadRequest;
            var inputErrors = ex.InputErrors;
            var customErrorCode = ex.CustomErrorCode;
            var customOverrideMessage = ex.Message;

            httpErrorCode = ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                ForbiddenException => HttpStatusCode.Forbidden,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                BadUserInputException => HttpStatusCode.BadRequest,
                _ => httpErrorCode
            };

            await WriteErrorResponseAsync(context, httpErrorCode, inputErrors, customErrorCode, customOverrideMessage);

            _logger.LogWarning(ex, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteErrorResponseAsync(
                context, HttpStatusCode.InternalServerError, null, CustomErrorCode.InternalServerErrorDefault, (ex is InternalSystemException)
                    ? ((InternalSystemException)ex).CustomOverrideMessage
                    : string.Empty);

            _logger.LogError(ex, ex.Message);
        }
    }

    private async Task WriteErrorResponseAsync(
        HttpContext context, HttpStatusCode code,
        IDictionary<string, object> inputErrors, CustomErrorCode customErrorCode,
        string customMessageOverride)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)code;

        await response
            .WriteAsync(JsonConvert.SerializeObject(new
            {
                error = new ErrorResponse
                {
                    Message = string.IsNullOrWhiteSpace(customMessageOverride) ? customErrorCode.GetEnumDescription() : customMessageOverride,
                    SplunkLink = _env.IsDevelopment() ? "Splunk logs are unavailable for local dev environment" : GetSplunkLink(context),
                    InputErrors = inputErrors,
                    Code = customErrorCode
                },
            }));
    }

    private string GetSplunkLink(HttpContext context)
    {
        var traceId = context?.TraceIdentifier;

        if (string.IsNullOrEmpty(traceId))
        {
            return string.Empty;
        }

        // add splunk search url to response
        var currentTime = DateTime.UtcNow - new DateTime(1970, 1, 1);
        var endTimeInSec = (int)currentTime.TotalSeconds + 5; // Add a little padding time, just in case
        var startTimeInSec = endTimeInSec - 600; // Assume the request took less than 10 minutes

        if (!_env.IsDevelopment() && !_splunkConfig.IsValidIndex)
        {
            _logger.LogError("Invalid settings provided for splunk link generation. SPLUNKLABEL=" +
                             $"{_splunkConfig.SplunkLabel}, SYSTEM_ENV={_splunkConfig.System_Env}");
        }

        return string.Format(Constants.Splunk.SplunkUrlTemplate, startTimeInSec, endTimeInSec, _splunkConfig.Index, traceId);
    }
}
