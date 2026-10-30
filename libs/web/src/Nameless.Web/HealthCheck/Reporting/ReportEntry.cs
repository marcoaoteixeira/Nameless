using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthCheck.Reporting;

/// <summary>
///     Report Entry
/// </summary>
public record ReportEntry {
    /// <summary>
    ///     Gets or sets the health status.
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the duration.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    ///     Gets or sets the tags.
    /// </summary>
    public IEnumerable<string>? Tags { get; set; }

    /// <summary>
    ///     Gets or sets the exception message.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    ///     Gets or sets the data.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Data { get; set; }

    /// <summary>
    ///     Creates a new <see cref="ReportEntry"/> instance.
    /// </summary>
    /// <param name="report">
    ///     The <see cref="HealthReportEntry"/> instance.
    /// </param>
    /// <returns>
    ///     A <see cref="ReportEntry"/> instance.
    /// </returns>
    public static ReportEntry Create(HealthReportEntry report) {
        return new ReportEntry {
            Status = report.Status,
            Description = report.Description,
            Duration = report.Duration,
            Tags = report.Tags,
            Exception = report.Exception?.Message,
            Data = report.Data
        };
    }
}