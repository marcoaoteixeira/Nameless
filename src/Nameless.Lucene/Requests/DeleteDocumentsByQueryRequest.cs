using Lucene.Net.Search;

namespace Nameless.Lucene.Requests;

/// <summary>
///     Represents a request to delete documents from a Lucene index.
/// </summary>
/// <param name="Query">
///     The query.
/// </param>
/// <param name="IndexName">
///     The index name.
/// </param>
public record DeleteDocumentsByQueryRequest(Query Query, string? IndexName = null)
    : RequestBase(IndexName);