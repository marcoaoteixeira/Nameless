using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json;

internal static class LoggerHighPerformanceExtension {
    #region Debug

    private static readonly Action<ILogger, string, Exception?> GettingResourceForCultureDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Getting resource for culture '{CultureName}'",
                                       options: null);

    internal static void GettingResourceForCulture(this ILogger<ResourceManager> self, string cultureName)
        => GettingResourceForCultureDelegate(self, cultureName, null /* exception */);

    private static readonly Action<ILogger, string, string, Exception?> GettingCurrentCultureFromContextDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Debug,
                                               eventId: default,
                                               formatString: "Getting current culture information from context. Context: '{Context}', Culture retrieved: '{CultureName}'",
                                               options: null);

    internal static void GettingCurrentCultureFromContext(this ILogger<CultureProvider> self, string context, string cultureName)
        => GettingCurrentCultureFromContextDelegate(self, context, cultureName, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> ResourceFileNotFoundDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Resource file not found. Resource path: '{ResourcePath}'",
                                       options: null);

    internal static void ResourceFileNotFound(this ILogger<ResourceManager> self, string resourcePath)
        => ResourceFileNotFoundDelegate(self, resourcePath, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> ResourceFileContentIsEmptyDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Resource file seems empty. Resource path: '{ResourcePath}'",
                                       options: null);

    internal static void ResourceFileContentIsEmpty(this ILogger<ResourceManager> self, string resourcePath)
        => ResourceFileContentIsEmptyDelegate(self, resourcePath, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> CreatingCacheEntryForResourceFileDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Creating new cache entry for resource file '{ResourcePath}'",
                                       options: null);

    internal static void CreatingCacheEntryForResourceFile(this ILogger<ResourceManager> self, string resourcePath)
        => CreatingCacheEntryForResourceFileDelegate(self, resourcePath, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> SettingFileWatcherForResourceFileDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Setting file watcher for resource file '{ResourcePath}'",
                                       options: null);

    internal static void SettingFileWatcherForResourceFile(this ILogger<ResourceManager> self, string resourcePath)
        => SettingFileWatcherForResourceFileDelegate(self, resourcePath, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> GettingResourceDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Getting resource '{ResourceName}'",
                                       options: null);

    internal static void GettingResource(this ILogger<StringLocalizerFactory> self, string resourceName)
        => GettingResourceDelegate(self, resourceName, null /* exception */);

    #endregion

    #region Information

    private static readonly Action<ILogger, string, Exception?> ResourceNotAvailableDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Resource not available. Resource name: {ResourceName}",
                                       options: null);

    internal static void ResourceNotAvailable(this ILogger<StringLocalizerFactory> self, string resourceName)
        => ResourceNotAvailableDelegate(self, resourceName, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> MessageNotFoundDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Message with id '{MessageId}' not found.",
                                       options: null);

    internal static void MessageNotFound(this ILogger<StringLocalizer> self, string messageId)
        => MessageNotFoundDelegate(self, messageId, null /* exception */);

    #endregion

    #region Error

    private static readonly Action<ILogger, string, Exception?> ErrorReadingResourceFileDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "An error occurs while reading resource file. Resource path: '{ResourcePath}'",
                                       options: null);

    internal static void ErrorReadingResourceFile(this ILogger<ResourceManager> self, string resourcePath, Exception exception)
        => ErrorReadingResourceFileDelegate(self, resourcePath, exception);

    private static readonly Action<ILogger, string, Exception?> ResourceDeserializationFailedDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "An error occurs while deserializing resource file JSON content. Resource path: '{ResourcePath}'",
                                       options: null);

    internal static void ResourceDeserializationFailed(this ILogger<ResourceManager> self, string resourcePath, Exception exception)
        => ResourceDeserializationFailedDelegate(self, resourcePath, exception);

    #endregion
}