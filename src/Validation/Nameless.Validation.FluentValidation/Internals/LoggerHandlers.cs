using Microsoft.Extensions.Logging;

namespace Nameless.Validation.FluentValidation.Internals;
internal static class LoggerHandlers {
    internal static readonly Action<ILogger,
        string,
        Exception?> ValidatorNotFound
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Validator not found for {Type}",
                                       options: null);
}
