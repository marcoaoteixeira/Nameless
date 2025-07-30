using Microsoft.Extensions.Logging;
using Nameless.Web.Filters;
using Nameless.Web.IdentityModel.Jwt;
using Nameless.Web.Infrastructure;

namespace Nameless.Web.Internals;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> MissingSecretConfigurationDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.MissingSecretConfigurationEvent,
            formatString: "The JSON Web Token (JWT) secret is not configured. Please ensure it is specified in the application settings."
        );

    private static readonly Action<ILogger, Exception?> MissingClaimSubDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.MissingClaimSubEvent,
            formatString: "The required claim 'sub' is missing from the claims list."
        );

    private static readonly Action<ILogger, Exception> CreateJsonWebTokenFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.CreateJsonWebTokenFailureEvent,
            formatString: "An error occurred while trying to create the JSON Web Token."
        );

    private static readonly Action<ILogger, Exception?> ValidationServiceUnavailableDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: Events.ValidationServiceUnavailableEvent,
            formatString: "Validation service is unavailable.");

    private static readonly Action<ILogger, Exception> RecurringTaskErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.RecurringTaskErrorEvent,
            formatString: "Error while executing recurring task.");

    internal static void MissingSecretConfiguration(this ILogger<JsonWebTokenProvider> self) {
        MissingSecretConfigurationDelegate(self, null /* exception */);
    }

    internal static void MissingClaimSub(this ILogger<JsonWebTokenProvider> self) {
        MissingClaimSubDelegate(self, null /* exception */);
    }

    internal static void CreateJsonWebTokenFailure(this ILogger<JsonWebTokenProvider> self, Exception exception) {
        CreateJsonWebTokenFailureDelegate(self, exception);
    }

    internal static void ValidationServiceUnavailable(this ILogger<ValidateRequestEndpointFilter> self) {
        ValidationServiceUnavailableDelegate(self, null /* exception */);
    }

    internal static void RecurringTaskError(this ILogger<RecurringHostService> self, Exception exception) {
        RecurringTaskErrorDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId MissingSecretConfigurationEvent = new(12001, nameof(MissingSecretConfiguration));
        internal static readonly EventId MissingClaimSubEvent = new(12002, nameof(MissingClaimSub));
        internal static readonly EventId CreateJsonWebTokenFailureEvent = new(12003, nameof(CreateJsonWebTokenFailure));
        internal static readonly EventId ValidationServiceUnavailableEvent = new(11002, nameof(ValidationServiceUnavailable));
        internal static readonly EventId RecurringTaskErrorEvent = new(11003, nameof(RecurringTaskError));
    }
}