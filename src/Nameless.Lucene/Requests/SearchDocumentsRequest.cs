using Lucene.Net.Search;

namespace Nameless.Lucene.Requests;

public sealed record SearchDocumentsRequest {
    public Query Query { get; init; }
    
    public Sort Sort { get; init; }
    
    public int Start { get; init; }
    
    public int Limit { get; init; }

    public SearchDocumentsRequest(Query? query = null, Sort? sort = null, int start = 0, int limit = 10) {
        Query = query ?? new MatchAllDocsQuery();
        Sort = sort ?? Sort.RELEVANCE;
        Start = start;
        Limit = limit;
    }
}