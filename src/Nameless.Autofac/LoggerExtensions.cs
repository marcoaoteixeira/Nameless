using Microsoft.Extensions.Logging;

namespace Nameless.Autofac;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> HostApplicationLifetimeUnavailableDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            eventId: default,
            formatString: "Service '{Service}' is unavailable. Autofac dispose handler will not be registered.");

    extension(ILogger logger) {
        internal void HostApplicationLifetimeUnavailable(string serviceName) {
            HostApplicationLifetimeUnavailableDelegate(logger, serviceName, arg3: null /* exception */);
        }
    }
}