using Microsoft.Extensions.Logging;

namespace Nameless.Web.Endpoints.Definitions;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> MissingEndpointRouteDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: Events.MissingEndpointRouteEvent,
            formatString: "Missing route for endpoint '{Endpoint}'.");

    internal static void MissingEndpointRoute(this ILogger<EndpointDescriptor> self, IEndpointDescriptor descriptor) {
        MissingEndpointRouteDelegate(self, descriptor.EndpointType.Name, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId MissingEndpointRouteEvent = new(11001, nameof(MissingEndpointRoute));
    }
}