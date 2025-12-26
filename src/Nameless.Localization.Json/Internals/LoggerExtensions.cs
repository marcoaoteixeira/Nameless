using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json.Internals;

/// <summary>
/// <see cref="ILogger"/> extensions.
/// </summary>
internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> GettingResourceForCultureDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Debug,
            Events.GettingResourceForCultureEvent,
            formatString: "Getting resource for culture '{CultureName}'");

    private static readonly Action<ILogger, string, Exception?> ResourceFileNotFoundDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            Events.ResourceFileNotFoundEvent,
            formatString: "Resource file not found. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ResourceFileContentIsEmptyDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            Events.ResourceFileContentIsEmptyEvent,
            formatString: "Resource file seems empty. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> CreatingCacheEntryForResourceFileDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Debug,
            Events.CreatingCacheEntryForResourceFileEvent,
            formatString: "Creating new cache entry for resource file '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> SettingFileWatcherForResourceFileDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Debug,
            Events.SettingFileWatcherForResourceFileEvent,
            formatString: "Setting file watcher for resource file '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ErrorReadingResourceFileDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            Events.ErrorReadingResourceFileEvent,
            formatString: "An error occurs while reading resource file. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ResourceDeserializationFailedDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            Events.ResourceDeserializationFailedEvent,
            formatString:
            "An error occurs while deserializing resource file JSON content. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, string, Exception?> GettingCurrentCultureFromContextDelegate
        = LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            Events.GettingCurrentCultureFromContextEvent,
            formatString:
            "Getting current culture information from context. Context: '{Context}', Culture retrieved: '{CultureName}'");

    private static readonly Action<ILogger, string, Exception?> GettingResourceDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Debug,
            Events.GettingResourceEvent,
            formatString: "Getting resource '{ResourceName}'");

    private static readonly Action<ILogger, string, Exception?> ResourceNotAvailableDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            Events.ResourceNotAvailableEvent,
            formatString: "Resource not available. Resource name: {ResourceName}");

    private static readonly Action<ILogger, string, Exception?> MessageNotFoundDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            Events.MessageNotFoundEvent,
            formatString: "Message with id '{MessageId}' not found.");

    extension(ILogger<ResourceManager> self) {
        internal void GettingResourceForCulture(string cultureName) {
            GettingResourceForCultureDelegate(self, cultureName, arg3: null /* exception */);
        }

        internal void ResourceFileNotFound(string resourcePath) {
            ResourceFileNotFoundDelegate(self, resourcePath, arg3: null /* exception */);
        }

        internal void ResourceFileContentIsEmpty(string resourcePath) {
            ResourceFileContentIsEmptyDelegate(self, resourcePath, arg3: null /* exception */);
        }

        internal void CreatingCacheEntryForResourceFile(string resourcePath) {
            CreatingCacheEntryForResourceFileDelegate(self, resourcePath, arg3: null /* exception */);
        }

        internal void SettingFileWatcherForResourceFile(string resourcePath) {
            SettingFileWatcherForResourceFileDelegate(self, resourcePath, arg3: null /* exception */);
        }

        internal void ErrorReadingResourceFile(string resourcePath, Exception exception) {
            ErrorReadingResourceFileDelegate(self, resourcePath, exception);
        }

        internal void ResourceDeserializationFailed(string resourcePath,
            Exception exception) {
            ResourceDeserializationFailedDelegate(self, resourcePath, exception);
        }
    }

    internal static void GettingCurrentCultureFromContext(this ILogger<CultureProvider> self, string context,
        string cultureName) {
        GettingCurrentCultureFromContextDelegate(self, context, cultureName, arg4: null /* exception */);
    }

    extension(ILogger<StringLocalizerFactory> self) {
        internal void GettingResource(string resourceName) {
            GettingResourceDelegate(self, resourceName, arg3: null /* exception */);
        }

        internal void ResourceNotAvailable(string resourceName) {
            ResourceNotAvailableDelegate(self, resourceName, arg3: null /* exception */);
        }
    }

    internal static void MessageNotFound(this ILogger<StringLocalizer> self, string messageId) {
        MessageNotFoundDelegate(self, messageId, arg3: null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId GettingResourceForCultureEvent =
            new(id: 4001, nameof(GettingResourceForCulture));

        internal static readonly EventId ResourceFileNotFoundEvent = new(id: 4002, nameof(ResourceFileNotFound));

        internal static readonly EventId ResourceFileContentIsEmptyEvent =
            new(id: 4003, nameof(ResourceFileContentIsEmpty));

        internal static readonly EventId CreatingCacheEntryForResourceFileEvent =
            new(id: 4004, nameof(CreatingCacheEntryForResourceFile));

        internal static readonly EventId SettingFileWatcherForResourceFileEvent =
            new(id: 4005, nameof(SettingFileWatcherForResourceFile));

        internal static readonly EventId
            ErrorReadingResourceFileEvent = new(id: 4006, nameof(ErrorReadingResourceFile));

        internal static readonly EventId ResourceDeserializationFailedEvent =
            new(id: 4007, nameof(ResourceDeserializationFailed));

        internal static readonly EventId GettingCurrentCultureFromContextEvent =
            new(id: 4008, nameof(GettingCurrentCultureFromContext));

        internal static readonly EventId GettingResourceEvent = new(id: 4009, nameof(GettingResource));
        internal static readonly EventId ResourceNotAvailableEvent = new(id: 4010, nameof(ResourceNotAvailable));
        internal static readonly EventId MessageNotFoundEvent = new(id: 4011, nameof(MessageNotFound));
    }
}