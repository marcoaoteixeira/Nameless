using Microsoft.Extensions.Logging;
using Nameless.MongoDB.Options;

namespace Nameless.MongoDB.Internals;

internal static class LoggerHighPerformanceExtension {
    private static readonly Action<ILogger, string, Exception?> MapperShouldInheritFromDocumentMapperBaseDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Debug,
                                       eventId: default,
                                       formatString: "\"{DocumentMapper}\" does not inherit from document mapping base type.",
                                       options: null);

    internal static void MapperShouldInheritFromDocumentMapperBase(this ILogger<MongoOptions> self, string type)
        => MapperShouldInheritFromDocumentMapperBaseDelegate(self, type, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> ErrorOnLoadingDocumentMapperTypeDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "An error occurs while trying to get type \"{DocumentMapper}\".",
                                       options: null);

    internal static void ErrorOnLoadingDocumentMapperType(this ILogger<MongoOptions> self, Exception exception, string type)
        => ErrorOnLoadingDocumentMapperTypeDelegate(self, type, exception);

    private static readonly Action<ILogger, string, Exception?> DocumentMapperTypeNotLoadedDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Unable to get type \"{DocumentMapper}\".",
                                       options: null);

    internal static void DocumentMapperTypeNotLoaded(this ILogger<MongoOptions> self, string type)
        => DocumentMapperTypeNotLoadedDelegate(self, type, null /* exception */);

    private static readonly Action<ILogger, string, Exception?> ErrorOnCreatingDocumentMapperDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Error,
                                       eventId: default,
                                       formatString: "An error occurs while trying to create document mapper instance for \"{DocumentMapper}\"",
                                       options: null);

    internal static void ErrorOnCreatingDocumentMapper(this ILogger<MongoOptions> self, Exception exception, Type type)
        => ErrorOnCreatingDocumentMapperDelegate(self, $"{type.FullName}, {type.Assembly.GetName().Name}", exception);
}