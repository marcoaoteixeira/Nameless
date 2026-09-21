using System.Diagnostics.CodeAnalysis;
using Serilog.Configuration;

namespace Nameless.Logging.Serilog;

/// <summary>
///     Registration options for the Serilog logging integration.
/// </summary>
[ExcludeFromCodeCoverage]
public class SerilogRegistration {
    /// <summary>
    ///     Whether it should overwrite the settings configuration.
    ///     When <see langword="true"/>, overwrite the settings configuration
    ///     using the delegate <see cref="ConfigureSettings"/>; otherwise
    ///     if the delegate is present it will append the extra configuration.
    /// </summary>
    public bool OverwriteSettingsConfiguration { get; private set; }

    /// <summary>
    ///     Gets the optional delegate to configure the settings.
    /// </summary>
    public Action<IServiceProvider, LoggerSettingsConfiguration>? ConfigureSettings { get; private set; }

    /// <summary>
    ///     Whether it should overwrite the enrichment configuration or just
    ///     append.
    /// </summary>
    public bool OverwriteEnrichmentConfiguration { get; private set; }

    /// <summary>
    ///     Gets the optional delegate to configure Serilog enrichment
    ///     options.
    /// </summary>
    public Action<IServiceProvider, LoggerEnrichmentConfiguration>? ConfigureEnrichment { get; private set; }

    /// <summary>
    ///     Whether it should overwrite the sink configuration or just append.
    /// </summary>
    public bool OverwriteSinkConfiguration { get; private set; }

    /// <summary>
    ///     Gets the optional delegate to configure Serilog sink
    ///     options.
    /// </summary>
    public Action<IServiceProvider, LoggerSinkConfiguration>? ConfigureSink { get; private set; }

    /// <summary>
    ///     Whether it should overwrite the minimum level configuration or
    ///     just append.
    /// </summary>
    public bool OverwriteMinimumLevelConfiguration { get; private set; }

    /// <summary>
    ///     Gets the optional delegate to configure Serilog minimum
    ///     level options.
    /// </summary>
    public Action<IServiceProvider, LoggerMinimumLevelConfiguration>? ConfigureMinimumLevel { get; private set; }

    /// <summary>
    ///     Sets delegate to configure the settings.
    /// </summary>
    /// <param name="value">
    ///     The delegate.
    /// </param>
    /// <param name="overwrite">
    ///     Whether it should overwrite the current configuration.
    /// </param>
    public SerilogRegistration WithSettingsConfiguration(Action<IServiceProvider, LoggerSettingsConfiguration> value, bool overwrite = false) {
        ConfigureSettings = value;
        OverwriteSettingsConfiguration = overwrite;

        return this;
    }

    /// <summary>
    ///     Sets the delegate to configure Serilog enrichment options.
    /// </summary>
    /// <param name="value">
    ///     The delegate.
    /// </param>
    /// <param name="overwrite">
    ///     Whether it should overwrite the current configuration.
    /// </param>
    public SerilogRegistration WithEnrichmentConfiguration(Action<IServiceProvider, LoggerEnrichmentConfiguration> value, bool overwrite = false) {
        ConfigureEnrichment = value;
        OverwriteEnrichmentConfiguration = overwrite;

        return this;
    }

    /// <summary>
    ///     Sets the delegate to configure Serilog sink options.
    /// </summary>
    /// <param name="value">
    ///     The delegate.
    /// </param>
    /// <param name="overwrite">
    ///     Whether it should overwrite the current configuration.
    /// </param>
    public SerilogRegistration WithSinkConfiguration(Action<IServiceProvider, LoggerSinkConfiguration> value, bool overwrite = false) {
        ConfigureSink = value;
        OverwriteSinkConfiguration = overwrite;

        return this;
    }

    /// <summary>
    ///     Sets the delegate to configure Serilog minimum level.
    /// </summary>
    /// <param name="value">
    ///     The delegate.
    /// </param>
    /// <param name="overwrite">
    ///     Whether it should overwrite the current configuration.
    /// </param>
    public SerilogRegistration WithMinimumLevelConfiguration(Action<IServiceProvider, LoggerMinimumLevelConfiguration> value, bool overwrite = false) {
        ConfigureMinimumLevel = value;
        OverwriteMinimumLevelConfiguration = overwrite;

        return this;
    }
}
