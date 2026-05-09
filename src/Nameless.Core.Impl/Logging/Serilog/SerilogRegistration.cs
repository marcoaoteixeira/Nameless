using Serilog;

namespace Nameless.Logging.Serilog;

/// <summary>
///     Registration options for the Serilog logging integration.
/// </summary>
public class SerilogRegistration {
    /// <summary>
    ///     Gets or sets an optional delegate to replace the default Serilog configuration
    ///     (console, file, and OpenTelemetry sinks) with a custom one.
    /// </summary>
    public Action<IServiceProvider, LoggerConfiguration>? OverrideSerilogConfiguration { get; set; }
}
