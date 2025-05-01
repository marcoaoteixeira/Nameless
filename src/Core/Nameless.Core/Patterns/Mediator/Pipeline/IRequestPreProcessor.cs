namespace Nameless.Patterns.Mediator.Pipeline;

public interface IRequestPreProcessor<in TRequest>
    where TRequest : notnull {
    Task ProcessAsync(TRequest request, CancellationToken cancellationToken);
}