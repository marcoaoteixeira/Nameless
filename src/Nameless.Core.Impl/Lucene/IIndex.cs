using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene;

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
    Result<bool> Insert(DocumentCollection documents);

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
    ///     Updates the document using the term as query to locate the document
    ///     in the index to be replaced.
    /// </summary>
    /// <param name="term">
    ///     The term.
    /// </param>
    /// <param name="document">
    ///     The document to update.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<bool> Update(Term term, Document document);

    /// <summary>
    ///     Searches documents in the index and returns an enumerable
    ///     that provides access to the documents found.
    /// </summary>
    /// <param name="query">
    ///     The query.
    /// </param>
    /// <param name="sort">
    ///     The sort option.
    /// </param>
    /// <param name="limit">
    ///     When searching, to avoid consume too much machine resources, set
    ///     the limit value for each interaction.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    IEnumerable<ScoreDocument> Search(Query query, Sort sort, int limit);

    /// <summary>
    ///     Retrieves the total number of documents in the index.
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
    ///     Rollback all changes done to the index.
    /// </summary>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<bool> Rollback();

    /// <summary>
    ///     Saves all changes done to the index.
    /// </summary>
    /// <returns>
    ///     A <see cref="Result{T}"/> object with information regarding
    ///     the execution.
    /// </returns>
    Result<bool> SaveChanges();
}