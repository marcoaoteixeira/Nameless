using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

public record SearchDocumentsMetadata(ISearchHit[] Hits, int Count) {
    public static SearchDocumentsMetadata Empty => new(Hits: [], Count: 0);
}

public class SearchDocumentsResponse : Result<SearchDocumentsMetadata> {
    private SearchDocumentsResponse(SearchDocumentsMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator SearchDocumentsResponse(SearchDocumentsMetadata value) {
        return new SearchDocumentsResponse(value, errors: []);
    }

    public static implicit operator SearchDocumentsResponse(Error error) {
        return new SearchDocumentsResponse(value: SearchDocumentsMetadata.Empty, errors: [error]);
    }

    public static Task<SearchDocumentsResponse> From(ISearchHit[] hits, int count) {
        return Task.FromResult<SearchDocumentsResponse>(
            new SearchDocumentsMetadata(hits, count)
        );
    }

    public static Task<SearchDocumentsResponse> From(Error error) {
        return Task.FromResult<SearchDocumentsResponse>(error);
    }
}