using Microsoft.Extensions.Logging;

namespace Nameless.Web.Internals;
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        Exception?> JwtValidationErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while validation JWT",
                               options: null);

    internal static void JwtValidationError(this ILogger self, Exception exception)
        => JwtValidationErrorHandler(self, exception);

    private static readonly Action<ILogger,
        string,
        object,
        Exception?> CantAddClaimHandler
        = LoggerMessage.Define<string,
            object>(logLevel: LogLevel.Warning,
                    eventId: default,
                    formatString: "Claim not added to token descriptor: {ClaimType} => {ClaimValue}",
                    options: null);

    internal static void CantAddClaim(this ILogger self, string claimType, object claimValue)
        => CantAddClaimHandler(self, claimType, claimValue, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> RecurringTaskErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while executing recurring task.",
                               options: null);

    internal static void RecurringTaskError(this ILogger self, Exception exception)
        => RecurringTaskErrorHandler(self, exception);
}
