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
    IDocument NewDocument(string documentID);

    /// <summary>
    /// Stores all documents into the index.
    /// </summary>
    /// <param name="documents">List of documents to store.</param>
    /// <returns>
    /// An <see cref="IndexActionResult"/> with information regards to this action.
    /// </returns>
    IndexActionResult StoreDocuments(IDocument[] documents);

    /// <summary>
    /// Removes all documents from the index.
    /// </summary>
    /// <param name="documents">List of documents to delete.</param>
    /// <returns>
    /// An <see cref="IndexActionResult"/> with information regards to this action.
    /// </returns>
    IndexActionResult DeleteDocuments(IDocument[] documents);

    /// <summary>
    /// Creates a search builder for this provider
    /// </summary>
    /// <returns>A search builder instance</returns>
    ISearchBuilder CreateSearchBuilder();
}