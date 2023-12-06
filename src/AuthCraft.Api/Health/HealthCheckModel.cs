using System;
using System.Collections.Generic;
using System.Net;

namespace AuthCraft.Api.Health;

public class HealthCheckModel
{
    public HealthCheckModel()
    {
        CurrentTime = DateTime.UtcNow;
    }

    public DateTime CurrentTime { get; }

    public long ExecutionTimeInMilliseconds { get; set; } = 0;

    public HttpStatusCode Status { get; internal set; }

    public IEnumerable<HealthIndicatorModel> HealthChecks { get; set; }

    public string AwsRegion => Environment.GetEnvironmentVariable("AWS_REGION");
}
