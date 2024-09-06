using Microsoft.Extensions.Logging;

namespace Nameless.Localization.Json.Internals;

internal static class LoggerHelper {
    internal static Action<ILogger, string, Exception?> TranslationFileNotFound = LoggerMessage.Define<string>(
        logLevel: LogLevel.Information,
        eventId: default,
        formatString: "Couldn't find translation file. File name: {FileName}",
        options: null
    );

    internal static Action<ILogger, string, Exception?> ReadTranslationFileContentFailure = LoggerMessage.Define<string>(
        logLevel: LogLevel.Error,
        eventId: default,
        formatString: "Error while reading translation file. File name: {FileName}",
        options: null
    );

    internal static Action<ILogger, string, Exception?> TranslationFileEmpty = LoggerMessage.Define<string>(
        logLevel: LogLevel.Warning,
        eventId: default,
        formatString: "Translation file seems empty. File name: {FileName}",
        options: null
    );

    internal static Action<ILogger, string, Exception?> TranslationObjectDeserializationFailure = LoggerMessage.Define<string>(
        logLevel: LogLevel.Error,
        eventId: default,
        formatString: "Error while deserializing JSON content to translation object. File name: {FileName}",
        options: null
    );

    internal static Action<ILogger, string, Exception?> JsonSerializerReturnNullTranslationObject = LoggerMessage.Define<string>(
        logLevel: LogLevel.Warning,
        eventId: default,
        formatString: "JSON serializer failed to deserialize translation object. File name: {FileName}",
        options: null
    );
}