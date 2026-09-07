using Microsoft.Extensions.Options;
using Nameless.Attributes;

namespace Nameless.Reporting;

/// <summary>
///     Defines the Status Reporting feature options.
/// </summary>
[ConfigurationSectionName("StatusReporting")]
public class StatusReportingOptions {
    /// <summary>
    ///     Gets or sets the number of most-recent status updates replayed to
    ///     new subscribers of each channel created by hub.
    /// </summary>
    public int BufferSize { get; set; } = 10;
}

internal static class StatusReportingOptionsExtensions {
    extension(IOptions<StatusReportingOptions> self) {
        internal IOptions<StatusReportingOptions> Validate() {

            Throws.When.LowerThan(self.Value.BufferSize, compare: 1);

            return self;
        }
    }
}