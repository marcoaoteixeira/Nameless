using Microsoft.Extensions.Logging;

namespace Nameless.Web.Endpoints;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MissingEndpointRouteDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.MissingEndpointRouteEvent,
            formatString: "Missing route for endpoint '{Endpoint}'.");

    internal static void MissingEndpointRoute(this ILogger<EndpointDescriptor> self, Type endpointType) {
        MissingEndpointRouteDelegate(self, endpointType.Name, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId MissingEndpointRouteEvent = new(11001, nameof(MissingEndpointRoute));
    }
}