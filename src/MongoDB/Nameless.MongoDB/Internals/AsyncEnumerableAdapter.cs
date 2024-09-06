using MongoDB.Driver;

namespace Nameless.MongoDB.Internals;

internal class AsyncEnumerableAdapter<T> : IAsyncEnumerable<T> {
    private readonly IAsyncCursorSource<T> _source;

    internal AsyncEnumerableAdapter(IAsyncCursorSource<T> source) {
        _source = Prevent.Argument.Null(source, nameof(source));
    }

    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
        => new AsyncEnumeratorAdapter<T>(_source, cancellationToken);
}