using Nameless.Mediator.Requests;

namespace Nameless.Mediator.Fixtures.Requests;

public struct StructResult {
    public int Value { get; set; }
}

public record OpenGenericRequest<TResult> : IRequest<TResult>
    where TResult : struct {
    public TResult Result { get; set; }
}

public class OpenGenericRequestHandler<TResult> : IRequestHandler<OpenGenericRequest<TResult>, TResult>
    where TResult : struct {
    public Task<TResult> HandleAsync(OpenGenericRequest<TResult> request, CancellationToken cancellationToken) {
        return Task.FromResult(request.Result);
    }
}

public class DayOfWeekCloseTypeRequestHandler : OpenGenericRequestHandler<DayOfWeek>;

public class IntegerCloseTypeRequestHandler : OpenGenericRequestHandler<int>;