using Microsoft.Extensions.Logging;

namespace Nameless.Web.Internals;
internal static class LoggerHandlers {
    internal static readonly Action<ILogger,
        Exception?> JwtValidationError
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while validation JWT",
                               options: null);

    internal static readonly Action<ILogger,
        string,
        object,
        Exception?> ClaimDescriptorSkipClaim
        = LoggerMessage.Define<string,
            object>(logLevel: LogLevel.Warning,
                    eventId: default,
                    formatString: "Claim not added to token descriptor: {ClaimType} => {ClaimValue}",
                    options: null);

    internal static readonly Action<ILogger,
        Exception?> RecurringTaskException
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while executing recurring task.",
                               options: null);
}
