using Nameless.Mediator.Requests;

namespace Nameless.Windows.UseCases.Restore;

public class PerformApplicationRestoreRequest : IRequest<PerformApplicationRestoreResponse> {
    public required DateTimeOffset Timestamp { get; init; }
}