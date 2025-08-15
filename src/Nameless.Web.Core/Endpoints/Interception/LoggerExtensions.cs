using Microsoft.Extensions.Logging;

namespace Nameless.Web.Endpoints.Interception;
internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, string, Exception> InterceptionFailureDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Error,
            eventId: Events.InterceptionFailureEvent,
            formatString: "An error occurred while intercepting endpoint '{Endpoint}' with interceptor '{Interceptor}'."
        );

    internal static void InterceptionFailure(this ILogger<EndpointInterceptorBase> self, Type endpointType, Type interceptorType, Exception exception) {
        InterceptionFailureDelegate(self, endpointType.Name, interceptorType.Name, exception);
    }

    internal static class Events {
        internal static readonly EventId InterceptionFailureEvent = new(11010, nameof(InterceptionFailure));
    }
}
