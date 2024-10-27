using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Infrastructure.Impl;

namespace Nameless.Localization.Json;

internal static class LoggerHighPerformanceExtension {
    #region Debug

    private static readonly Action<ILogger, string, Exception?> GettingTranslationForCultureDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Getting translation for culture '{CultureName}'",
                                       options: null);

    internal static void GettingTranslationForCulture(this ILogger<TranslationManager> self, string cultureName)
        => GettingTranslationForCultureDelegate(self, cultureName, null /* exception */);

    private static readonly Action<ILogger, string, string, Exception?> GettingCurrentCultureFromContextDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Debug,
                                               eventId: default,
                                               formatString: "Getting current culture information from context. Context: '{Context}', Culture retrieved: '{CultureName}'",
                                               options: null);

    internal static void GettingCurrentCultureFromContext(this ILogger<CultureContext> self, string context, string cultureName)
        => GettingCurrentCultureFromContextDelegate(self, context, cultureName, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> TranslationFileNotFoundDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Translation file not found. File name: '{FileName}'",
                                       options: null);

    internal static void TranslationFileNotFound(this ILogger<TranslationManager> self, string fileName)
        => TranslationFileNotFoundDelegate(self, fileName, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> TranslationFileContentIsEmptyDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Translation file seems empty. File name: '{FileName}'",
                                       options: null);

    internal static void TranslationFileContentIsEmpty(this ILogger<TranslationManager> self, string fileName)
        => TranslationFileContentIsEmptyDelegate(self, fileName, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> CreatingCacheEntryForTranslationFileDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Creating new cache entry for translation file '{FilePath}'",
                                       options: null);

    internal static void CreatingCacheEntryForTranslationFile(this ILogger<TranslationManager> self, string path)
        => CreatingCacheEntryForTranslationFileDelegate(self, path, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> SettingFileWatcherForTranslationFileDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Setting file watcher for translation file '{FileName}'",
                                       options: null);

    internal static void SettingFileWatcherForTranslationFile(this ILogger<TranslationManager> self, string fileName)
        => SettingFileWatcherForTranslationFileDelegate(self, fileName, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> GettingRegionFromTranslationObjectDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "Getting translation region '{Region}'",
                                       options: null);

    internal static void GettingRegionFromTranslationObject(this ILogger<StringLocalizerFactory> self, string region)
        => GettingRegionFromTranslationObjectDelegate(self, region, null /* exception */);

    #endregion

    #region Information

    private static readonly Action<ILogger, string, Exception?> RegionNotFoundInTranslationObjectDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Region not found in translation object. Region: '{RegionName}'",
                                       options: null);

    internal static void RegionNotFoundInTranslationObject(this ILogger<StringLocalizerFactory> self, string regionName)
        => RegionNotFoundInTranslationObjectDelegate(self, regionName, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> TranslationNotFoundForMessageWithIDInformationDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Translation not found for message with ID: '{MessageID}'",
                                       options: null);

    internal static void TranslationNotFoundForMessageWithID(this ILogger<StringLocalizer> self, string messageID)
        => TranslationNotFoundForMessageWithIDInformationDelegate(self, messageID, null /* exception */);

    #endregion

    #region Error

    private static readonly Action<ILogger, string, Exception?> ErrorReadingTranslationFileDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "An error occurs while reading translation file. File name: '{FileName}'",
                                       options: null);

    internal static void ErrorReadingTranslationFile(this ILogger<TranslationManager> self, string fileName, Exception exception)
        => ErrorReadingTranslationFileDelegate(self, fileName, exception);

    private static readonly Action<ILogger, string, Exception?> TranslationObjectDeserializationFailedDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "An error occurs while trying to deserialize translation file JSON content to translation object. File name: '{FileName}'",
                                       options: null);

    internal static void TranslationObjectDeserializationFailed(this ILogger<TranslationManager> self, string fileName, Exception exception)
        => TranslationObjectDeserializationFailedDelegate(self, fileName, exception);

    #endregion

    #region Warning

    private static readonly Action<ILogger, string, Exception?> JsonSerializerReturnedNullTranslationObjectDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "JSON serializer returned null translation object for unknown reason. File name: '{FileName}'",
                                       options: null);

    internal static void JsonDeserializationReturnedNullTranslationObject(this ILogger<TranslationManager> self, string fileName)
        => JsonSerializerReturnedNullTranslationObjectDelegate(self, fileName, null /* exception */);

    #endregion
}