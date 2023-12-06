using System.Threading.Tasks;

namespace AuthCraft.Api.Health;

/// <summary>
/// Specification for a health check. Implementations of this interface are automatically loaded
/// by the IoC container.
/// </summary>
public interface IHealthIndicator
{
    /// <summary>
    /// Name of the indicator describing what it is indicating to return to a client
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Perform the concrete health check.
    /// </summary>
    /// <returns></returns>
    Task<HealthIndicatorModel> CheckStatusAsync();
}
