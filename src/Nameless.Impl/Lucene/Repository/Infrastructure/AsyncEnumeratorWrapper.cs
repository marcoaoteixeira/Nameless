using Nameless.Lucene.ObjectModel;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository.Infrastructure;

public class AsyncEnumeratorWrapper<TEntity> : IAsyncEnumerator<TEntity>
    where TEntity : class {
    private readonly IEnumerable<ScoreDocument> _enumerable;
    private readonly IMapper _mapper;
    private readonly CancellationToken _cancellationToken;

    public TEntity Current => GetCurrent();

    private IEnumerator<ScoreDocument> Inner {
        get => _enumerator ??= _enumerable.GetEnumerator();
    }

    private IEnumerator<ScoreDocument>? _enumerator;
    private TEntity? _current;
    private bool _disposed;

    public AsyncEnumeratorWrapper(IEnumerable<ScoreDocument> enumerable, IMapper mapper, CancellationToken cancellationToken) {
        _enumerable = enumerable;
        _mapper = mapper;
        _cancellationToken = cancellationToken;
    }

    public ValueTask<bool> MoveNextAsync() {
        BlockAccessAfterDispose();

        _cancellationToken.ThrowIfCancellationRequested();

        var result = Inner.MoveNext();

        if (result) {
            _current = _mapper.Map<TEntity>(Inner.Current);
        }

        return ValueTask.FromResult(result);
    }

    public ValueTask DisposeAsync() {
        if (_disposed) { return ValueTask.CompletedTask; }

        _enumerator?.Dispose();

        _enumerator = null;
        _disposed = true;

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private TEntity GetCurrent() {
        return _current ?? throw new InvalidOperationException("Enumerator is not initialized.");
    }
}