using Microsoft.Extensions.Logging;

namespace Nameless.Localization.Json;

/// <summary>
/// <see cref="ILogger"/> extensions.
/// </summary>
internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> GettingResourceDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: Events.GettingResourceEvent,
            formatString: "Getting resource '{ResourceName}'");

    private static readonly Action<ILogger, string, Exception?> ResourceNotAvailableDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.ResourceNotAvailableEvent,
            formatString: "Resource not available. Resource name: {ResourceName}");

    private static readonly Action<ILogger, string, Exception?> MessageNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.MessageNotFoundEvent,
            formatString: "Message with id '{MessageId}' not found.");

    internal static void GettingResource(this ILogger<StringLocalizerFactory> self, string resourceName) {
        GettingResourceDelegate(self, resourceName, null /* exception */);
    }

    internal static void ResourceNotAvailable(this ILogger<StringLocalizerFactory> self, string resourceName) {
        ResourceNotAvailableDelegate(self, resourceName, null /* exception */);
    }

    internal static void MessageNotFound(this ILogger<StringLocalizer> self, string messageId) {
        MessageNotFoundDelegate(self, messageId, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId GettingResourceEvent = new(4009, nameof(GettingResource));
        internal static readonly EventId ResourceNotAvailableEvent = new(4010, nameof(ResourceNotAvailable));
        internal static readonly EventId MessageNotFoundEvent = new(4011, nameof(MessageNotFound));
    }
}