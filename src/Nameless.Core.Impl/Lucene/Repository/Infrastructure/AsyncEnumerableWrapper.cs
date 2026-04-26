using Nameless.Lucene.ObjectModel;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository.Infrastructure;

public class AsyncEnumerableWrapper<TEntity> : IAsyncEnumerable<TEntity>
    where TEntity : class {
    private readonly IEnumerable<ScoreDocument> _inner;
    private readonly IMapper _mapper;

    public AsyncEnumerableWrapper(IEnumerable<ScoreDocument> inner, IMapper mapper) {
        _inner = inner;
        _mapper = mapper;
    }

    public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
        return new AsyncEnumeratorWrapper<TEntity>(_inner, _mapper, cancellationToken);
    }
}