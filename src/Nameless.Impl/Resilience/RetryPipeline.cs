using Polly;

namespace Nameless.Resilience;

public class RetryPipeline : IRetryPipeline {
    private readonly ResiliencePipeline _inner;

    public static RetryPipeline Empty => new(
        tag: $"{Guid.CreateVersion7():N}",
        ResiliencePipeline.Empty
    );

    public string? Tag { get; }

    public RetryPipeline(string? tag, ResiliencePipeline inner) {
        _inner = inner;

        Tag = tag;
    }

    public ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> operation, CancellationToken cancellationToken) {
        return _inner.ExecuteAsync(operation, cancellationToken);
    }

    public ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> operation, CancellationToken cancellationToken) {
        return _inner.ExecuteAsync(operation, cancellationToken);
    }
}
