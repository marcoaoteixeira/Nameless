using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Contracts;

namespace Nameless.Web.HealthCheck.Reporting;

/// <summary>
///     Report
/// </summary>
public record Report {
    /// <summary>
    ///     Gets the heath check status.
    /// </summary>
    public HealthStatus Status { get; }

    /// <summary>
    ///     Gets the execution duration.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    ///     Gets the entries
    /// </summary>
    public IReadOnlyDictionary<string, ReportEntry> Entries { get; private set; }

    private Report(HealthStatus status, TimeSpan duration, IReadOnlyDictionary<string, ReportEntry> entries) {
        Status = status;
        Duration = duration;
        Entries = entries;
    }

    /// <summary>
    ///     Creates a report from a <see cref="HealthReport"/> instance.
    /// </summary>
    /// <param name="report">
    ///     The <see cref="HealthReport"/> instance.
    /// </param>
    /// <returns>
    ///     A <see cref="Report"/> instance.
    /// </returns>
    public static Report Create(HealthReport report) {
        Throws.When.Null(report);

        var entries = report.Entries.ToDictionary(
            entry => entry.Key,
            entry => ReportEntry.Create(entry.Value)
        );

        return new Report(report.Status, report.TotalDuration, entries);
    }
}