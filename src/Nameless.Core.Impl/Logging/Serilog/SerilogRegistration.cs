using Serilog;

namespace Nameless.Logging.Serilog;

public class SerilogRegistration {
    public Action<IServiceProvider, LoggerConfiguration>? OverrideSerilogConfiguration { get; set; }
}
