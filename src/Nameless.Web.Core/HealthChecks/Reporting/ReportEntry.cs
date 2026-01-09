using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthChecks.Reporting;

public record ReportEntry {
    public HealthStatus Status { get; set; }
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public string? Exception { get; set; }
    public IReadOnlyDictionary<string, object>? Data { get; set; }

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