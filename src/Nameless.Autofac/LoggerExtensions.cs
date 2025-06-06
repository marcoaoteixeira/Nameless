using Microsoft.Extensions.Logging;

namespace Nameless.Autofac;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> HostApplicationLifetimeUnavailableDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: new EventId((int)Events.HostApplicationLifetimeUnavailable, nameof(Events.HostApplicationLifetimeUnavailable)),
            formatString: "Service {Service} is unavailable. Autofac dispose handler will not be registered.");

    internal static void HostApplicationLifetimeUnavailable(this ILogger logger, Type serviceType) {
        HostApplicationLifetimeUnavailableDelegate(logger, serviceType.Name, null /* exception */);
    }

    private enum Events {
        HostApplicationLifetimeUnavailable = 1
    }
}
