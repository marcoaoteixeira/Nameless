namespace Nameless.Lucene.Requests;

/// <summary>
///     Represents a request to include documents into a Lucene index.
/// </summary>
/// <typeparam name="TDocument">
///     Type of the document.
/// </typeparam>
/// <param name="Documents">
///     The documents.
/// </param>
/// <param name="IndexName">
///     The index name.
/// </param>
public record InsertDocumentsRequest<TDocument>(TDocument[] Documents, string? IndexName = null) : RequestBase(IndexName)
    where TDocument : class;