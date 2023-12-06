using System.Diagnostics;
using System.Threading.Tasks;

namespace AuthCraft.Api.Health;

public abstract class BaseHealthIndicator : IHealthIndicator
{
    private Stopwatch _stopwatch;

    public abstract string Identifier { get; }

    public abstract bool Verbose { get; }

    protected long ElapsedTimeImMilliseconds
    {
        get
        {
            return _stopwatch?.ElapsedMilliseconds ?? -1;
        }
    }

    public abstract Task<HealthIndicatorModel> CheckStatusAsync();

    protected void StartTimeMeasurement()
    {
        StopTimeMeasurement();
        _stopwatch = Stopwatch.StartNew();
    }

    protected void StopTimeMeasurement()
    {
        _stopwatch?.Stop();
    }
}
