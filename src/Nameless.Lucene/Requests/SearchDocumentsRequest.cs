using Lucene.Net.Search;

namespace Nameless.Lucene.Requests;

public record SearchDocumentsRequest(string? IndexName = null) : RequestBase(IndexName) {
    public required Query Query { get; init; }

    public Sort Sort { get; init; } = Sort.RELEVANCE;

    public int Start { get; init; } = 0;

    public int Limit { get; init; } = 10;
}