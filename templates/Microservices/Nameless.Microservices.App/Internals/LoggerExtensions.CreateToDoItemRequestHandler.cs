using Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

namespace Nameless.Microservices.App.Internals;

internal static partial class LoggerExtensions {
    private static readonly Action<ILogger, Exception> CreateToDoItemFailedDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while creating ToDo item."
        );

    extension(ILogger<CreateToDoItemRequestHandler> self) {
        internal void CreateToDoItemFailed(Exception exception) {
            CreateToDoItemFailedDelegate(self, exception);
        }
    }
}
