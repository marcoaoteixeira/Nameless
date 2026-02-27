namespace Nameless.Lucene.Requests;

/// <summary>
///     Represents a request to delete documents from a Lucene index.
/// </summary>
/// <param name="IDs">
///     The documents' ID.
/// </param>
/// <param name="IndexName">
///     The index name.
/// </param>
public record DeleteDocumentsRequest(object[] IDs, string? IndexName = null)
    : RequestBase(IndexName);