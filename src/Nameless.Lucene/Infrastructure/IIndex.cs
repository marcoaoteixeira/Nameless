using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Infrastructure;

/// <summary>
///     Defines an index.
/// </summary>
public interface IIndex : IDisposable {
    /// <summary>
    ///     Gets the index name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Inserts the documents into the index.
    /// </summary>
    /// <param name="documents">
    ///     The documents.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<int> Insert(DocumentCollection documents);

    /// <summary>
    ///     Deletes the documents from the index.
    /// </summary>
    /// <param name="query">
    ///     The query.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<bool> Delete(Query query);

    /// <summary>
    ///     Searches documents in the index.
    /// </summary>
    /// <param name="query">
    ///     The query.
    /// </param>
    /// <param name="collector">
    ///     The collector.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<DocumentCollection> Search(Query query, ICollector collector);

    /// <summary>
    ///     Counts documents in the index.
    /// </summary>
    /// <param name="query">
    ///     The query.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<int> Count(Query query);

    /// <summary>
    ///     Saves changes made to the index.
    /// </summary>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<bool> SaveChanges();
}