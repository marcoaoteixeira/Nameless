using System.Linq.Expressions;

namespace Nameless.Testing.Tools.Mockers.EntityFrameworkCore;

public class InMemoryAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T> {
    private readonly Lazy<IQueryProvider> _provider;

    public IQueryProvider Provider => _provider.Value;

    public InMemoryAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable) {
        _provider = new Lazy<IQueryProvider>(CreateQueryProvider);
    }

    public InMemoryAsyncEnumerable(Expression expression)
        : base(expression) {
        _provider = new Lazy<IQueryProvider>(CreateQueryProvider);
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
        return new InMemoryAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    public IAsyncEnumerator<T> GetEnumerator() {
        return GetAsyncEnumerator();
    }

    private InMemoryAsyncQueryProvider<T> CreateQueryProvider() {
        return new InMemoryAsyncQueryProvider<T>(this);
    }
}
