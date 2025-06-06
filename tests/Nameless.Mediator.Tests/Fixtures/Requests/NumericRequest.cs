using Nameless.Mediator.Requests;

namespace Nameless.Mediator.Fixtures.Requests;
public record NumericRequest : IRequest<double> {
    public double X { get; set; }
    public double Y { get; set; }
}

public class SumNumericRequestHandler : IRequestHandler<NumericRequest, double> {
    public Task<double> HandleAsync(NumericRequest request, CancellationToken cancellationToken) {
        return Task.FromResult(request.X + request.Y);
    }
}

public class SubtractNumericRequestHandler : IRequestHandler<NumericRequest, double> {
    public Task<double> HandleAsync(NumericRequest request, CancellationToken cancellationToken) {
        return Task.FromResult(request.X - request.Y);
    }
}

public class MultiplyNumericRequestHandler : IRequestHandler<NumericRequest, double> {
    public Task<double> HandleAsync(NumericRequest request, CancellationToken cancellationToken) {
        return Task.FromResult(request.X * request.Y);
    }
}