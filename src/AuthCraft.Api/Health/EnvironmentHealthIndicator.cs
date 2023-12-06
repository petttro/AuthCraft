using System;
using System.Net;
using System.Threading.Tasks;
using AuthCraft.Common.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthCraft.Api.Health;

public class EnvironmentHealthIndicator : BaseHealthIndicator
{
    private readonly ILogger _logger;
    private readonly EnvironmentConfig _envConfig;

    public EnvironmentHealthIndicator(ILogger<EnvironmentHealthIndicator> logger, IOptions<EnvironmentConfig> envOptions)
    {
        _logger = logger;
        _envConfig = envOptions.Value;
    }

    public override string Identifier => "Environment Configuration";

    public override bool Verbose => false;

    public override async Task<HealthIndicatorModel> CheckStatusAsync()
    {
        try
        {
            var label = Environment.GetEnvironmentVariable("LABEL");

            if (string.IsNullOrWhiteSpace(label))
            {
                label = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            }

            StartTimeMeasurement();

            var model = new HealthIndicatorModel
            {
                Name = Identifier,
                Details = label
            };

            model.AddResult(GetBuildTypeModel());
            model.AddResult(GetDotnetCoreModel());
            model.AddResult(GetBuildInfoModel());
            model.AddResult(GetClusterStackInfo());

            StopTimeMeasurement();

            return await Task.FromResult(model);
        }
        catch (Exception ex)
        {
            StopTimeMeasurement();

            _logger.LogError(ex, "Unhandled Exception during HealthIndicator Check");

            return await Task.FromResult(new HealthIndicatorModel
            {
                Status = HttpStatusCode.InternalServerError,
                Details = "Unhandled Exception during EnvironmentHealthIndicator check: " + ex,
                Name = Identifier,
                ExecutionTimeInMilliseconds = ElapsedTimeImMilliseconds
            });
        }
    }

    private HealthIndicatorModel GetDotnetCoreModel()
    {
        return new HealthIndicatorModel(".Net Core Version", HttpStatusCode.OK, Environment.Version.ToString())
        {
            ExecutionTimeInMilliseconds = ElapsedTimeImMilliseconds
        };
    }

    private HealthIndicatorModel GetClusterStackInfo()
    {
        var stackInfo = Environment.GetEnvironmentVariable("HOSTLABEL") ?? "Unknown";

        return new HealthIndicatorModel("Cluster Stack Information", HttpStatusCode.OK, stackInfo);
    }

    private HealthIndicatorModel GetBuildInfoModel()
    {
        var version = _envConfig.BuildVersion;

        version = version == "BAMBOO_WILL_REPLACE_THIS_VALUE" ? "Local Development" : version.Replace("~", "__");

        var model = new HealthIndicatorModel("Build Version", HttpStatusCode.OK, version);
        model.ExecutionTimeInMilliseconds = ElapsedTimeImMilliseconds;

        return model;
    }

    private HealthIndicatorModel GetBuildTypeModel()
    {
        var model = new HealthIndicatorModel("Build Type", HttpStatusCode.ServiceUnavailable, "UNKNOWN");

#if DEBUG
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        model = new HealthIndicatorModel("Build Type", environment == "Development" ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, "DEBUG");
#endif

#if RELEASE
            model = new HealthIndicatorModel("Build Type", HttpStatusCode.OK, "RELEASE");
#endif

        model.ExecutionTimeInMilliseconds = ElapsedTimeImMilliseconds;

        return model;
    }
}
