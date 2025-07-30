﻿using Microsoft.Extensions.Logging;

namespace Nameless.Autofac.Internals;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> HostApplicationLifetimeUnavailableDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.HostApplicationLifetimeUnavailableEvent,
            formatString: "Service '{Service}' is unavailable. Autofac dispose handler will not be registered.");

    internal static void HostApplicationLifetimeUnavailable(this ILogger logger, string serviceName) {
        HostApplicationLifetimeUnavailableDelegate(logger, serviceName, null /* exception */);
    }

    internal static class Events {
        internal static EventId HostApplicationLifetimeUnavailableEvent = new(2001, nameof(HostApplicationLifetimeUnavailable));
    }
}
