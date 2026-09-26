using Nameless.Microservices.Api.Domains.Chores.UseCases.Create;
using Nameless.Microservices.Api.Domains.Chores.UseCases.Delete;
using Nameless.Microservices.Api.Domains.Chores.UseCases.MarkDone;

namespace Nameless.Microservices.Api.Domains.Chores;

internal static class LoggerExtensions {
    extension(ILogger<CreateChoreEndpoint> self) {
        internal void Failure(Exception ex) {
            Log.Failure(self, "CREATE-CHORE", nameof(CreateChoreEndpoint.HandleAsync), ex);
        }
    }

    extension(ILogger<DeleteChoreEndpoint> self) {
        internal void Failure(Exception ex) {
            Log.Failure(self, "DELETE-CHORE", nameof(DeleteChoreEndpoint.HandleAsync), ex);
        }
    }

    extension(ILogger<MarkDoneEndpoint> self) {
        internal void Failure(Exception ex) {
            Log.Failure(self, "MARK-DONE", nameof(MarkDoneEndpoint.HandleAsync), ex);
        }
    }
}
