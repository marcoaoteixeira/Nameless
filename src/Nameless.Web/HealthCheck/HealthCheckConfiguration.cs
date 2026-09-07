using Nameless.Attributes;

namespace Nameless.Web.HealthCheck;

/// <summary>
///     Health check configuration options.
/// </summary>
[ConfigurationSectionName("HealthCheck")]
public record HealthCheckConfiguration {
    /// <summary>
    ///     Gets the internal timeout policy name for the health checks
    /// </summary>
    internal const string TimeoutPolicyName = "__HealthCheck_Timeout_Policy__";

    /// <summary>
    ///     Gets or sets the health check endpoint request timeout.
    ///     Default is <c>5</c> seconds.
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Gets or sets the health check endpoint output cache timeout.
    ///     Default is <c>10</c> seconds.
    /// </summary>
    public TimeSpan OutputCacheTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Gets or sets the liveness route path. Default is <c>health</c>.
    /// </summary>
    public string LivenessPath { get; set; } = "health";

    /// <summary>
    ///     Gets or sets the readiness route path. Default is <c>ready</c>.
    /// </summary>
    public string ReadinessPath { get; set; } = "ready";

    /// <summary>
    ///     Gets or sets the readiness tags.
    /// </summary>
    public string[] ReadinessTags { get; set; } = [];
}