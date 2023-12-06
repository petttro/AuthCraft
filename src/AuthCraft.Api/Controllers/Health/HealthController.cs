using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuthCraft.Api.Health;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthCraft.Api.Controllers.Health;

[AllowAnonymous]
[Route("[controller]")]
[Produces("application/json")]
public class HealthController : BaseController
{
    private readonly ILogger _logger;
    private readonly IEnumerable<IHealthIndicator> _indicators;

    public HealthController(IEnumerable<IHealthIndicator> indicators, ILoggerFactory loggerFactory)
    {
        _indicators = indicators;
        _logger = loggerFactory.CreateLogger<HealthController>();
    }

    [ProducesResponseType(typeof(HealthCheckModel), (int)HttpStatusCode.OK)]
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var healthTasks = _indicators
            .OrderBy(x => x.Identifier)
            .Select(healthIndicator =>
            {
                try
                {
                    return healthIndicator.CheckStatusAsync();
                }
                catch (Exception ex)
                {
                    var message = $"Exception={ex}";
                    _logger.LogError(message, ex);

                    return Task.FromResult(new HealthIndicatorModel
                    {
                        Name = healthIndicator.Identifier,
                        Details = message,
                        Status = HttpStatusCode.ServiceUnavailable,
                        ExecutionTimeInMilliseconds = -1,
                    });
                }
            })
            .ToArray();

        await Task.WhenAll(healthTasks);

        var status = healthTasks.Length > 0 ? healthTasks.Max(x => x.Result.Status) : HttpStatusCode.OK;
        var executionTime = healthTasks.Length > 0 ? healthTasks.Max(x => x.Result.ExecutionTimeInMilliseconds) : 0;

        var result = new HealthCheckModel
        {
            Status = status,

            // Order results so that problems are at the top of the list
            HealthChecks = healthTasks
                .Select(x => x.Result)
                .OrderByDescending(x => (int)x.Status)
                .ThenBy(x => x.Name),
            ExecutionTimeInMilliseconds = executionTime,
        };

        return StatusCode(200, result);
    }
}
