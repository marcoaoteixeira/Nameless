using System.Diagnostics.CodeAnalysis;
using Serilog.Configuration;

namespace Nameless.Logging.Serilog;

/// <summary>
///     Registration options for the Serilog logging integration.
/// </summary>
[ExcludeFromCodeCoverage]
public class SerilogRegistration {
    /// <summary>
    ///     Whether it should override the enrichment configuration or just
    ///     append.
    /// </summary>
    public bool OverrideEnrichmentConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets an optional delegate to configure Serilog enrichment
    ///     options.
    /// </summary>
    public Action<IServiceProvider, LoggerEnrichmentConfiguration>? EnrichmentConfiguration { get; set; }

    /// <summary>
    ///     Whether it should override the sink configuration or just append.
    /// </summary>
    public bool OverrideSinkConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets an optional delegate to configure Serilog sink
    ///     options.
    /// </summary>
    public Action<IServiceProvider, LoggerSinkConfiguration>? SinkConfiguration { get; set; }

    /// <summary>
    ///     Whether it should override the minimum level configuration or
    ///     just append.
    /// </summary>
    public bool OverrideMinimumLevelConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets an optional delegate to configure Serilog minimum
    ///     level options.
    /// </summary>
    public Action<IServiceProvider, LoggerMinimumLevelConfiguration>? MinimumLevelConfiguration { get; set; }
}
