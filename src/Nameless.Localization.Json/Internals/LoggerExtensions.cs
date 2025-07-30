using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json.Internals;

/// <summary>
/// <see cref="ILogger"/> extensions.
/// </summary>
internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> GettingResourceForCultureDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: Events.GettingResourceForCultureEvent,
            formatString: "Getting resource for culture '{CultureName}'");

    private static readonly Action<ILogger, string, Exception?> ResourceFileNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.ResourceFileNotFoundEvent,
            formatString: "Resource file not found. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ResourceFileContentIsEmptyDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.ResourceFileContentIsEmptyEvent,
            formatString: "Resource file seems empty. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> CreatingCacheEntryForResourceFileDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: Events.CreatingCacheEntryForResourceFileEvent,
            formatString: "Creating new cache entry for resource file '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> SettingFileWatcherForResourceFileDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: Events.SettingFileWatcherForResourceFileEvent,
            formatString: "Setting file watcher for resource file '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ErrorReadingResourceFileDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: Events.ErrorReadingResourceFileEvent,
            formatString: "An error occurs while reading resource file. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ResourceDeserializationFailedDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: Events.ResourceDeserializationFailedEvent,
            formatString: "An error occurs while deserializing resource file JSON content. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, string, Exception?> GettingCurrentCultureFromContextDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Debug,
            eventId: Events.GettingCurrentCultureFromContextEvent,
            formatString: "Getting current culture information from context. Context: '{Context}', Culture retrieved: '{CultureName}'");

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

    internal static void GettingResourceForCulture(this ILogger<ResourceManager> self, string cultureName) {
        GettingResourceForCultureDelegate(self, cultureName, null /* exception */);
    }

    internal static void ResourceFileNotFound(this ILogger<ResourceManager> self, string resourcePath) {
        ResourceFileNotFoundDelegate(self, resourcePath, null /* exception */);
    }

    internal static void ResourceFileContentIsEmpty(this ILogger<ResourceManager> self, string resourcePath) {
        ResourceFileContentIsEmptyDelegate(self, resourcePath, null /* exception */);
    }

    internal static void CreatingCacheEntryForResourceFile(this ILogger<ResourceManager> self, string resourcePath) {
        CreatingCacheEntryForResourceFileDelegate(self, resourcePath, null /* exception */);
    }

    internal static void SettingFileWatcherForResourceFile(this ILogger<ResourceManager> self, string resourcePath) {
        SettingFileWatcherForResourceFileDelegate(self, resourcePath, null /* exception */);
    }

    internal static void ErrorReadingResourceFile(this ILogger<ResourceManager> self, string resourcePath, Exception exception) {
        ErrorReadingResourceFileDelegate(self, resourcePath, exception);
    }

    internal static void ResourceDeserializationFailed(this ILogger<ResourceManager> self, string resourcePath,
        Exception exception) {
        ResourceDeserializationFailedDelegate(self, resourcePath, exception);
    }

    internal static void GettingCurrentCultureFromContext(this ILogger<CultureProvider> self, string context,
        string cultureName) {
        GettingCurrentCultureFromContextDelegate(self, context, cultureName, null /* exception */);
    }

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
        internal static readonly EventId GettingResourceForCultureEvent = new(4001, nameof(GettingResourceForCulture));
        internal static readonly EventId ResourceFileNotFoundEvent = new(4002, nameof(ResourceFileNotFound));
        internal static readonly EventId ResourceFileContentIsEmptyEvent = new(4003, nameof(ResourceFileContentIsEmpty));
        internal static readonly EventId CreatingCacheEntryForResourceFileEvent = new(4004, nameof(CreatingCacheEntryForResourceFile));
        internal static readonly EventId SettingFileWatcherForResourceFileEvent = new(4005, nameof(SettingFileWatcherForResourceFile));
        internal static readonly EventId ErrorReadingResourceFileEvent = new(4006, nameof(ErrorReadingResourceFile));
        internal static readonly EventId ResourceDeserializationFailedEvent = new(4007, nameof(ResourceDeserializationFailed));
        internal static readonly EventId GettingCurrentCultureFromContextEvent = new(4008, nameof(GettingCurrentCultureFromContext));
        internal static readonly EventId GettingResourceEvent = new(4009, nameof(GettingResource));
        internal static readonly EventId ResourceNotAvailableEvent = new(4010, nameof(ResourceNotAvailable));
        internal static readonly EventId MessageNotFoundEvent = new(4011, nameof(MessageNotFound));
    }
}