using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthChecks.Reporting;

public record Report {
    public HealthStatus Status { get; private set; } = HealthStatus.Healthy;
    public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
    public Dictionary<string, ReportEntry> Entries { get; private set; } = [];

    public static Report Create(HealthReport report) {
        Throws.When.Null(report);

        var entries = report.Entries.ToDictionary(
            entry => entry.Key,
            entry => ReportEntry.Create(entry.Value)
        );

        return new Report { Status = report.Status, Duration = report.TotalDuration, Entries = entries };
    }
}