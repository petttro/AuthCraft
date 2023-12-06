using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace AuthCraft.Api.Health;

public class HealthIndicatorModel
{
    private HttpStatusCode _status;

    private long _executionTimeInMilliseconds;

    public HealthIndicatorModel()
    {
    }

    public HealthIndicatorModel(string name, HttpStatusCode status, string details)
        : this()
    {
        Name = name;
        Status = status;
        Details = details;
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Details { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IList<HealthIndicatorModel> Results { get; set; }

    public string Name { get; set; }

    public HttpStatusCode Status
    {
        get
        {
            return Results == null || !Results.Any()
                ? _status
                : Results.Max(x => x.Status);
        }

        set => _status = value;
    }

    public long ExecutionTimeInMilliseconds
    {
        get
        {
            return Results == null || !Results.Any()
                ? _executionTimeInMilliseconds
                : Results.Max(x => x.ExecutionTimeInMilliseconds);
        }

        set => _executionTimeInMilliseconds = value;
    }

    public void AddResult(HealthIndicatorModel result)
    {
        if (Results == null)
        {
            Results = new List<HealthIndicatorModel>();
        }

        Results.Add(result);
    }
}
