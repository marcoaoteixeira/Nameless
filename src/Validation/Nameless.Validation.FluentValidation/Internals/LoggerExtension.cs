using Microsoft.Extensions.Logging;

namespace Nameless.Validation.FluentValidation.Internals;
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        string,
        Exception?> ValidatorNotFoundForHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Validator not found for {Type}",
                                       options: null);

    internal static void ValidatorNotFoundFor<TType>(this ILogger self)
        => ValidatorNotFoundForHandler(self, typeof(TType).Name, null /* exception */);
}
