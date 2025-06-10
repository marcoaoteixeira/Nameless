using Microsoft.Extensions.Logging;

namespace Nameless.Web.Endpoints;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MissingEndpointRouteDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Missing route for endpoint '{Endpoint}'.");

    internal static void MissingEndpointRoute(this ILogger<EndpointBuilder> self, Type endpointType) {
        MissingEndpointRouteDelegate(self, endpointType.Name, null /* exception */);
    }
}