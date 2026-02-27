using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json.Internals;

/// <summary>
///     <see cref="ILogger"/> extension methods.
/// </summary>
internal static class LoggerExtensions {
    #region ResourceManager

    private static readonly Action<ILogger, string, Exception?> GettingResourceForCultureDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Getting resource for culture '{CultureName}'");

    private static readonly Action<ILogger, string, Exception?> ResourceFileNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Resource file not found. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ResourceFileContentIsEmptyDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Resource file seems empty. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> CreatingCacheEntryForResourceFileDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Creating new cache entry for resource file '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> SettingFileWatcherForResourceFileDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Setting file watcher for resource file '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ErrorReadingResourceFileDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurs while reading resource file. Resource path: '{ResourcePath}'");

    private static readonly Action<ILogger, string, Exception?> ResourceDeserializationFailedDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString:
            "An error occurs while deserializing resource file JSON content. Resource path: '{ResourcePath}'");

    #endregion
    
    #region CultureProvider

    private static readonly Action<ILogger, string, string, Exception?> GettingCurrentCultureFromContextDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString:
            "Getting current culture information from context. Context: '{Context}', Culture retrieved: '{CultureName}'");

    #endregion

    #region StringLocalizerFactory

    private static readonly Action<ILogger, string, Exception?> GettingResourceDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Getting resource '{ResourceName}'");

    private static readonly Action<ILogger, string, Exception?> ResourceNotAvailableDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Resource not available. Resource name: {ResourceName}");

    #endregion

    #region StringLocalizer

    private static readonly Action<ILogger, string, Exception?> MessageNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Message with id '{MessageId}' not found.");

    #endregion

    extension(ILogger<ResourceManager> self) {
        internal void GettingResourceForCulture(string cultureName) {
            GettingResourceForCultureDelegate(self, cultureName, null /* exception */);
        }

        internal void ResourceFileNotFound(string resourcePath) {
            ResourceFileNotFoundDelegate(self, resourcePath, null /* exception */);
        }

        internal void ResourceFileContentIsEmpty(string resourcePath) {
            ResourceFileContentIsEmptyDelegate(self, resourcePath, null /* exception */);
        }

        internal void CreatingCacheEntryForResourceFile(string resourcePath) {
            CreatingCacheEntryForResourceFileDelegate(self, resourcePath, null /* exception */);
        }

        internal void SettingFileWatcherForResourceFile(string resourcePath) {
            SettingFileWatcherForResourceFileDelegate(self, resourcePath, null /* exception */);
        }

        internal void ErrorReadingResourceFile(string resourcePath, Exception exception) {
            ErrorReadingResourceFileDelegate(self, resourcePath, exception);
        }

        internal void ResourceDeserializationFailed(string resourcePath, Exception exception) {
            ResourceDeserializationFailedDelegate(self, resourcePath, exception);
        }
    }

    extension(ILogger<CultureProvider> self) {
        internal void GettingCurrentCultureFromContext(string context, string cultureName) {
            GettingCurrentCultureFromContextDelegate(self, context, cultureName, null /* exception */);
        }
    }

    extension(ILogger<StringLocalizerFactory> self) {
        internal void GettingResource(string resourceName) {
            GettingResourceDelegate(self, resourceName, null /* exception */);
        }

        internal void ResourceNotAvailable(string resourceName) {
            ResourceNotAvailableDelegate(self, resourceName, null /* exception */);
        }
    }

    extension(ILogger<StringLocalizer> self) {
        internal void MessageNotFound(string messageId) {
            MessageNotFoundDelegate(self, messageId, null /* exception */);
        }
    }
}