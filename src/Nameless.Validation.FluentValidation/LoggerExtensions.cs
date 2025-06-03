using Microsoft.Extensions.Logging;

namespace Nameless.Validation.FluentValidation;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MissingValidatorForTypeDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Information,
            default,
            "Validator not found for {Type}");

    internal static void MissingValidatorForType<TType>(this ILogger self) {
        MissingValidatorForTypeDelegate(self, typeof(TType).Name, null /* exception */);
    }
}