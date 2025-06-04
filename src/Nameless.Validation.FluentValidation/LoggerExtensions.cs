using Microsoft.Extensions.Logging;

namespace Nameless.Validation.FluentValidation;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MissingValidatorForTypeDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Information,
            default,
            "Validator not found for type '{Type}'");

    internal static void MissingValidatorForType(this ILogger<ValidationService> self, object instance) {
        MissingValidatorForTypeDelegate(self, instance.GetType().Name, null /* exception */);
    }
}