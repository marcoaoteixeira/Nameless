using Microsoft.Extensions.Logging;

namespace Nameless.Localization.Json.Internals;

internal static class LoggerExtension {
    private static readonly Action<ILogger,
        string,
        Exception?> TranslationFileNotFoundHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Couldn't find translation file. File name: {FileName}",
                                       options: null);

    internal static void TranslationFileNotFound(this ILogger self, string fileName)
        => TranslationFileNotFoundHandler(self, fileName, null /* exception */);

    private static readonly Action<ILogger,
        string,
        Exception?> ReadTranslationFileContentErrorHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "Error while reading translation file. File name: {FileName}",
                                       options: null);

    internal static void ReadTranslationFileContentError(this ILogger self, string fileName, Exception exception)
        => ReadTranslationFileContentErrorHandler(self, fileName, exception);

    private static readonly Action<ILogger,
        string,
        Exception?> TranslationFileEmptyHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Translation file seems empty. File name: {FileName}",
                                       options: null);

    internal static void TranslationFileEmpty(this ILogger self, string fileName)
        => TranslationFileEmptyHandler(self, fileName, null /* exception */);

    private static readonly Action<ILogger,
        string,
        Exception?> TranslationObjectDeserializationErrorHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "Error while deserializing JSON content to translation object. File name: {FileName}",
                                       options: null);

    internal static void TranslationObjectDeserializationError(this ILogger self, string fileName, Exception exception)
        => TranslationObjectDeserializationErrorHandler(self, fileName, exception);

    private static readonly Action<ILogger,
        string,
        Exception?> JsonSerializerReturnNullTranslationObjectHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "JSON serializer failed to deserialize translation object. File name: {FileName}",
                                       options: null);

    internal static void JsonSerializerReturnNullTranslationObject(this ILogger self, string fileName)
        => JsonSerializerReturnNullTranslationObjectHandler(self, fileName, null /* exception */);
}