using Nameless.Lucene.ObjectModel;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository.Infrastructure;

/// <summary>
///     Represents an <see cref="IAsyncEnumerable{T}"/> for Lucene entities.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the entity.
/// </typeparam>
public class AsyncEnumerableWrapper<TEntity> : IAsyncEnumerable<TEntity>
    where TEntity : class {
    private readonly IEnumerable<ScoreDocument> _inner;
    private readonly IMapper _mapper;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AsyncEnumerableWrapper{TEntity}"/> class.
    /// </summary>
    /// <param name="inner">
    ///     The inner enumerable.
    /// </param>
    /// <param name="mapper">
    ///     The mapper.
    /// </param>
    public AsyncEnumerableWrapper(IEnumerable<ScoreDocument> inner, IMapper mapper) {
        _inner = inner;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
        return new AsyncEnumeratorWrapper<TEntity>(_inner, _mapper, cancellationToken);
    }
}