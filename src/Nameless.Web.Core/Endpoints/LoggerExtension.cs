using Microsoft.Extensions.Logging;

namespace Nameless.Web.Endpoints;
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        string,
        Exception?> EndpointMissingRoutePatternHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Endpoint {Endpoint} missing route pattern.",
                                       options: null);

    internal static void EndpointMissingRoutePattern(this ILogger self, EndpointBase endpoint)
        => EndpointMissingRoutePatternHandler(self, endpoint.GetType().Name, null /* exception */);

    private static readonly Action<ILogger,
        string,
        string,
        Exception?> EndpointNotMappedHandler
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Warning,
                                               eventId: default,
                                               formatString: "Could not map endpoint {Endpoint} with HTTP method {HttpMethod}.",
                                               options: null);

    internal static void EndpointNotMapped(this ILogger self, EndpointBase endpoint)
        => EndpointNotMappedHandler(self, endpoint.GetType().Name, endpoint.HttpMethod, null /* exception */);

    private static readonly Action<ILogger,
        string,
        string,
        Exception> AcceptHeaderItemErrorHandler
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Error,
                                               eventId: default,
                                               formatString: "Could not add accept header request for request type of {RequestType} with content-type {ContentType}.",
                                               options: null);

    internal static void AcceptHeaderItemError(this ILogger self, Accept accept, Exception exception)
        => AcceptHeaderItemErrorHandler(self, accept.RequestType?.Name ?? string.Empty, accept.ContentType, exception);
}
