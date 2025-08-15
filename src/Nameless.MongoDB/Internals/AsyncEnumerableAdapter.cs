using MongoDB.Driver;

namespace Nameless.MongoDB.Internals;

/// <summary>
/// An adapter for <see cref="IAsyncCursorSource{T}"/> to <see cref="IAsyncEnumerator{T}"/>.
/// </summary>
/// <typeparam name="TValue">Type of the value.</typeparam>
internal class AsyncEnumerableAdapter<TValue> : IAsyncEnumerable<TValue> {
    private readonly IAsyncCursorSource<TValue> _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncEnumerableAdapter{TValue}"/> class.
    /// </summary>
    /// <param name="source">The source of the async cursor.</param>
    internal AsyncEnumerableAdapter(IAsyncCursorSource<TValue> source) {
        _source = Guard.Against.Null(source);
    }

    /// <inheritdoc />
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator(CancellationToken cancellationToken) {
        return new AsyncEnumeratorAdapter<TValue>(_source, cancellationToken);
    }
}