using Microsoft.Extensions.Logging;

namespace Nameless.Validation.FluentValidation.Internals;
internal static class LoggerExtension {
    private static readonly Action<ILogger, string, Exception?> MissingValidatorForTypeDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Validator not found for {Type}",
                                       options: null);

    internal static void MissingValidatorForType<TType>(this ILogger self)
        => MissingValidatorForTypeDelegate(self, typeof(TType).Name, null /* exception */);
}
