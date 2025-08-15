﻿using Microsoft.Extensions.Logging;
using Nameless.Mediator.Events;

namespace Nameless.Mediator.Internals;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MissingEventHandlerDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: Events.MissingEventHandlerEvent,
            formatString: "Event handler not found for event '{EventType}'"
        );

    internal static void MissingEventHandler(this ILogger<EventHandlerWrapper> self, IEvent evt) {
        MissingEventHandlerDelegate(self, evt.GetType().Name, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId MissingEventHandlerEvent = new(6001, nameof(MissingEventHandler));
    }
}
