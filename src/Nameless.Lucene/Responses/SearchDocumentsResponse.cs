using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public sealed record SearchResult(ISearchHit[] Hits, int Count) {
    public static SearchResult Empty => new(Hits: [], Count: 0);
}

public sealed class SearchDocumentsResponse : Result<SearchResult> {
    private SearchDocumentsResponse(SearchResult value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator SearchDocumentsResponse(SearchResult value) {
        return new SearchDocumentsResponse(value, errors: []);
    }

    public static implicit operator SearchDocumentsResponse(Error error) {
        return new SearchDocumentsResponse(value: SearchResult.Empty, errors: [error]);
    }
}