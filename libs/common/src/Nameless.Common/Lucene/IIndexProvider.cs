namespace Nameless.Lucene;

/// <summary>
///     Represents a provider of <see cref="IIndex" /> instances.
/// </summary>
public interface IIndexProvider : IDisposable {
    /// <summary>
    ///     Retrieves an <see cref="IIndex" /> instance by its name.
    /// </summary>
    /// <param name="indexName">
    ///     The name of the index.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IIndex" />.
    /// </returns>
    IIndex Get(string? indexName);

    /// <summary>
    ///     Flushes and closes every currently-open index (commits pending changes —
    ///     disposing an IndexWriter in Lucene.NET 4.8 commits, it does not roll back;
    ///     call <see cref="IIndex.Rollback"/> first if uncommitted writes must be
    ///     discarded — then disposes the underlying IndexWriter/IndexReader/FSDirectory),
    ///     without disposing the provider itself. A later <see cref="Get"/> call
    ///     transparently reopens the index on demand.
    /// </summary>
    /// <remarks>
    ///     Any <see cref="IIndex"/> obtained before this call is disposed by it; using
    ///     it afterward throws <see cref="ObjectDisposedException"/> — call
    ///     <see cref="Get"/> again for a fresh instance. Do not call this while an
    ///     operation (including an in-progress <c>Search</c> enumeration) is using an
    ///     index obtained from this provider — <see cref="IIndex"/> is not thread-safe
    ///     and this method does not change that. Calling this after <see cref="IDisposable.Dispose"/>
    ///     throws <see cref="ObjectDisposedException"/>. Idempotent otherwise.
    /// </remarks>
    void CloseAll();
}