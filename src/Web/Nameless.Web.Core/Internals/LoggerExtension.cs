using Microsoft.Extensions.Logging;
using Nameless.Web.Endpoints;
using Nameless.Web.Filters;
using Nameless.Web.Infrastructure;
using Nameless.Web.Services;

namespace Nameless.Web;

internal static class LoggerExtension {
    private static readonly Action<ILogger, Exception> JwtValidationErrorDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while validation JWT",
                               options: null);

    internal static void JwtValidationError(this ILogger<JwtService> self, Exception exception)
        => JwtValidationErrorDelegate(self, exception);

    private static readonly Action<ILogger, string, object, Exception?> ClaimNotAddedWarningDelegate
        = LoggerMessage.Define<string, object>(logLevel: LogLevel.Warning,
                                               eventId: default,
                                               formatString: "Claim not added to token descriptor: {ClaimType} => {ClaimValue}",
                                               options: null);

    internal static void ClaimNotAddedWarning(this ILogger<JwtService> self, string claimType, object claimValue)
        => ClaimNotAddedWarningDelegate(self, claimType, claimValue, null /* exception */);

    private static readonly Action<ILogger, Exception> RecurringTaskErrorDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while executing recurring task.",
                               options: null);

    internal static void RecurringTaskError(this ILogger<RecurringTaskHostedService> self, Exception exception)
        => RecurringTaskErrorDelegate(self, exception);

    private static readonly Action<ILogger, string, Exception?> EndpointMissingRoutePatternWarningDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Endpoint {Endpoint} missing route pattern.",
                                       options: null);

    internal static void EndpointMissingRoutePatternWarning(this ILogger self, MinimalEndpointBase endpoint)
        => EndpointMissingRoutePatternWarningDelegate(self, endpoint.GetType().Name, null /* exception */);

    private static readonly Action<ILogger, string, string, Exception?> EndpointNotMappedWarningDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Warning,
                                               eventId: default,
                                               formatString: "Could not map endpoint {Endpoint} with HTTP method {HttpMethod}.",
                                               options: null);

    internal static void EndpointNotMappedWarning(this ILogger self, MinimalEndpointBase endpoint)
        => EndpointNotMappedWarningDelegate(self, endpoint.GetType().Name, endpoint.HttpMethod, null /* exception */);

    private static readonly Action<ILogger, string, string, Exception> EndpointMappingErrorDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Error,
                                               eventId: default,
                                               formatString: "Error while mapping endpoint {Endpoint} with HTTP method {HttpMethod}.",
                                               options: null);

    internal static void EndpointMappingError(this ILogger self, MinimalEndpointBase endpoint, Exception exception)
        => EndpointMappingErrorDelegate(self, endpoint.GetType().Name, endpoint.HttpMethod, exception);

    private static readonly Action<ILogger, string, string, Exception> EndpointConfigurationErrorDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Error,
                                               eventId: default,
                                               formatString: "Error while configuring endpoint {Endpoint} with HTTP method {HttpMethod}.",
                                               options: null);

    internal static void EndpointConfigurationError(this ILogger self, MinimalEndpointBase endpoint,
                                                    Exception exception)
        => EndpointConfigurationErrorDelegate(self, endpoint.GetType().Name, endpoint.HttpMethod, exception);

    private static readonly Action<ILogger, Exception?> ValidationServiceNotFoundWarningDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Missing dependency: Validation endpoint filter enabled but no validation service was found.",
                               options: null);

    internal static void ValidationServiceNotFoundWarning(this ILogger<ValidateEndpointFilter> self)
        => ValidationServiceNotFoundWarningDelegate(self, null /* exception */);
}