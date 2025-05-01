namespace Nameless.Patterns.Mediator.Pipeline;

public interface IRequestPostProcessor<in TRequest, in TResponse>
    where TRequest : notnull {
    Task ProcessAsync(TRequest request, TResponse response, CancellationToken cancellationToken);
}