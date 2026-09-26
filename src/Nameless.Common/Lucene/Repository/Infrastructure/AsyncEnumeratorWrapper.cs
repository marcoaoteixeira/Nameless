using Nameless.Lucene.ObjectModel;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository.Infrastructure;

/// <summary>
///     A wrapper for <see cref="IAsyncEnumerator{T}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     The type of the Lucene entity.
/// </typeparam>
public class AsyncEnumeratorWrapper<TEntity> : IAsyncEnumerator<TEntity>
    where TEntity : class {
    private readonly IEnumerable<ScoreDocument> _enumerable;
    private readonly IMapper _mapper;
    private readonly CancellationToken _cancellationToken;

    /// <inheritdoc />
    public TEntity Current => GetCurrent();

    private IEnumerator<ScoreDocument> Inner {
        get => _enumerator ??= _enumerable.GetEnumerator();
    }

    private IEnumerator<ScoreDocument>? _enumerator;
    private TEntity? _current;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AsyncEnumeratorWrapper{TEntity}"/> class.
    /// </summary>
    /// <param name="enumerable">
    ///     The enumerable to wrap.
    /// </param>
    /// <param name="mapper">
    ///     The mapper.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    public AsyncEnumeratorWrapper(IEnumerable<ScoreDocument> enumerable, IMapper mapper, CancellationToken cancellationToken) {
        _enumerable = enumerable;
        _mapper = mapper;
        _cancellationToken = cancellationToken;
    }

    /// <inheritdoc />
    public ValueTask<bool> MoveNextAsync() {
        BlockAccessAfterDispose();

        _cancellationToken.ThrowIfCancellationRequested();

        var result = Inner.MoveNext();

        if (result) {
            _current = _mapper.Map<TEntity>(Inner.Current);
        }

        return ValueTask.FromResult(result);
    }

    /// <inheritdoc />
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