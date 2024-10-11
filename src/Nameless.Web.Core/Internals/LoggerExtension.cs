// ReSharper disable InconsistentNaming

using Microsoft.Extensions.Logging;

namespace Nameless.Web;

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
        Exception?> ClaimNotAddedWarningHandler
        = LoggerMessage.Define<string,
            object>(logLevel: LogLevel.Warning,
                    eventId: default,
                    formatString: "Claim not added to token descriptor: {ClaimType} => {ClaimValue}",
                    options: null);

    internal static void ClaimNotAddedWarning(this ILogger self, string claimType, object claimValue)
        => ClaimNotAddedWarningHandler(self, claimType, claimValue, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> RecurringTaskErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while executing recurring task.",
                               options: null);

    internal static void RecurringTaskError(this ILogger self, Exception exception)
        => RecurringTaskErrorHandler(self, exception);

    private static readonly Action<ILogger,
        string,
        Exception?> EndpointMissingRoutePatternWarningHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Endpoint {Endpoint} missing route pattern.",
                                       options: null);

    internal static void EndpointMissingRoutePatternWarning(this ILogger self, MinimalEndpointBase endpoint)
        => EndpointMissingRoutePatternWarningHandler(self, endpoint.GetType().Name, null /* exception */);

    private static readonly Action<ILogger,
        string,
        string,
        Exception?> EndpointNotMappedWarningHandler
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Warning,
                                               eventId: default,
                                               formatString: "Could not map endpoint {Endpoint} with HTTP method {HttpMethod}.",
                                               options: null);

    internal static void EndpointNotMappedWarning(this ILogger self, MinimalEndpointBase endpoint)
        => EndpointNotMappedWarningHandler(self, endpoint.GetType().Name, endpoint.HttpMethod, null /* exception */);

    private static readonly Action<ILogger,
        string,
        string,
        Exception> EndpointMappingErrorHandler
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Error,
                                               eventId: default,
                                               formatString: "Error while mapping endpoint {Endpoint} with HTTP method {HttpMethod}.",
                                               options: null);

    internal static void EndpointMappingError(this ILogger self, MinimalEndpointBase endpoint, Exception exception)
        => EndpointMappingErrorHandler(self, endpoint.GetType().Name, endpoint.HttpMethod, exception);

    private static readonly Action<ILogger,
        string,
        string,
        Exception> EndpointConfigurationErrorHandler
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Error,
                                               eventId: default,
                                               formatString: "Error while configuring endpoint {Endpoint} with HTTP method {HttpMethod}.",
                                               options: null);

    internal static void EndpointConfigurationError(this ILogger self, MinimalEndpointBase endpoint,
                                                    Exception exception)
        => EndpointConfigurationErrorHandler(self, endpoint.GetType().Name, endpoint.HttpMethod, exception);

    private static readonly Action<ILogger,
        Exception?> ValidationServiceNotFoundWarningHandler
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Missing dependency: Validation endpoint filter enabled but no validation service was found.",
                               options: null);

    internal static void ValidationServiceNotFoundWarning(this ILogger self)
        => ValidationServiceNotFoundWarningHandler(self, null /* exception */);
}