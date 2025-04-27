namespace Nameless.Search;

/// <summary>
/// Contract to an index.
/// </summary>
public interface IIndex {
    /// <summary>
    /// Gets the name of the index.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether it is empty or not.
    /// </summary>
    bool IsEmpty();

    /// <summary>
    /// Gets the number of documents in this index.
    /// </summary>
    int Count();

    /// <summary>
    /// Creates a new empty document.
    /// </summary>
    /// <returns>
    /// A <see cref="IDocument"/> object.
    /// </returns>
    IDocument NewDocument(string documentID);

    /// <summary>
    /// Stores all documents into the index.
    /// </summary>
    /// <param name="documents">List of documents to store.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the store action.
    /// </returns>
    Task<IndexActionResult> StoreDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken);

    /// <summary>
    /// Removes all documents from the index.
    /// </summary>
    /// <param name="documents">List of documents to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the delete action.
    /// </returns>
    Task<IndexActionResult> DeleteDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a search builder for this provider
    /// </summary>
    /// <returns>A search builder instance</returns>
    ISearchBuilder CreateSearchBuilder();
}