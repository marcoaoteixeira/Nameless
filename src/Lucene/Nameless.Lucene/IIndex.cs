namespace Nameless.Lucene;

/// <summary>
/// Defines methods for an index representation.
/// </summary>
public interface IIndex {
    /// <summary>
    /// Gets the name of the index.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether an index is empty or not
    /// </summary>
    bool IsEmpty();

    /// <summary>
    /// Gets the number of indexed documents
    /// </summary>
    int CountDocuments();

    /// <summary>
    /// Creates an empty document
    /// </summary>
    /// <returns></returns>
    IIndexDocument NewDocument(string documentID);

    /// <summary>
    /// Stores all documents into the index.
    /// </summary>
    /// <param name="documents">List of documents to store.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the store documents action.
    /// </returns>
    Task<IndexActionResult> StoreDocumentsAsync(IIndexDocument[] documents, CancellationToken cancellationToken);

    /// <summary>
    /// Removes all documents from the index.
    /// </summary>
    /// <param name="documents">List of documents to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the delete documents action.
    /// </returns>
    Task<IndexActionResult> DeleteDocumentsAsync(IIndexDocument[] documents, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a search builder for this provider
    /// </summary>
    /// <returns>A search builder instance</returns>
    ISearchBuilder CreateSearchBuilder();
}