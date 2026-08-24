using System.Diagnostics.CodeAnalysis;
using Serilog.Configuration;

namespace Nameless.Logging.Serilog;

/// <summary>
///     Registration options for the Serilog logging integration.
/// </summary>
[ExcludeFromCodeCoverage]
public class SerilogRegistration {
    /// <summary>
    ///     Whether it should override the settings configuration.
    ///     When <see langword="true"/>, override the settings configuration
    ///     using the delegate <see cref="ConfigureSettings"/>; otherwise
    ///     if the delegate is present it will append the extra configuration.
    /// </summary>
    public bool OverrideSettingsConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets delegate to configure the settings.
    /// </summary>
    public Action<IServiceProvider, LoggerSettingsConfiguration>? ConfigureSettings { get; set; }

    /// <summary>
    ///     Whether it should override the enrichment configuration or just
    ///     append.
    /// </summary>
    public bool OverrideEnrichmentConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets an optional delegate to configure Serilog enrichment
    ///     options.
    /// </summary>
    public Action<IServiceProvider, LoggerEnrichmentConfiguration>? ConfigureEnrichment { get; set; }

    /// <summary>
    ///     Whether it should override the sink configuration or just append.
    /// </summary>
    public bool OverrideSinkConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets an optional delegate to configure Serilog sink
    ///     options.
    /// </summary>
    public Action<IServiceProvider, LoggerSinkConfiguration>? ConfigureSink { get; set; }

    /// <summary>
    ///     Whether it should override the minimum level configuration or
    ///     just append.
    /// </summary>
    public bool OverrideMinimumLevelConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets an optional delegate to configure Serilog minimum
    ///     level options.
    /// </summary>
    public Action<IServiceProvider, LoggerMinimumLevelConfiguration>? ConfigureMinimumLevel { get; set; }
}
