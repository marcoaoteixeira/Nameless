using Lucene.Net.Index;
using Lucene.Net.Search;
using Nameless.Lucene.Collections;
using Nameless.Lucene.Infrastructure;
using Nameless.Lucene.Internals;
using Nameless.Lucene.Mapping;
using Nameless.Lucene.Requests;
using Nameless.Lucene.Responses;
using Nameless.ObjectModel;
using ListExtensions = Lucene.Net.Util.ListExtensions;

namespace Nameless.Lucene.Infrastructure.Implementations;

public class Repository : IRepository {
    private const string REQUEST_CANCELLED_ERROR = "Requested task was cancelled.";
    private const string MAX_BOOLEAN_CLAUSE_EXCEEDED_ERROR = "The number of clauses in the boolean query exceeds the maximum allowed (1024).";

    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly EntityDocumentBuilder _entityDocumentBuilder;
    private readonly IIndexProvider _indexProvider;

    public Repository(IAnalyzerProvider analyzerProvider, EntityDocumentBuilder entityDocumentBuilder, IIndexProvider indexProvider) {
        _analyzerProvider = analyzerProvider;
        _entityDocumentBuilder = entityDocumentBuilder;
        _indexProvider = indexProvider;
    }

    /// <inheritdoc />
    public IQueryBuilder CreateQueryBuilder(string? indexName) {
        return new QueryBuilder(
            analyzer: _analyzerProvider.GetAnalyzer(indexName)
        );
    }

    /// <inheritdoc />
    public Task<InsertDocumentsResponse> InsertAsync<TDocument>(InsertDocumentsRequest<TDocument> request, CancellationToken cancellationToken)
        where TDocument : class {
        var docs = request.Documents
                          .TakeWhile(_ => !cancellationToken.IsCancellationRequested)
                          .Select(_entityDocumentBuilder.Build);

        if (cancellationToken.IsCancellationRequested) {
            return InsertDocumentsResponse.From(
                Error.Failure(REQUEST_CANCELLED_ERROR)
            );
        }

        var index = _indexProvider.Get(request.IndexName);

        var insert = index.Insert([.. docs]);

        if (insert.Failure) {
            return InsertDocumentsResponse.From(insert.Errors);
        }

        var save = index.SaveChanges();

        return save.Match(
            onSuccess: _ => InsertDocumentsResponse.From(request.Documents.Length),
            onFailure: InsertDocumentsResponse.From
        );
    }

    /// <inheritdoc />
    public Task<DeleteDocumentsResponse> DeleteAsync(DeleteDocumentsRequest request, CancellationToken cancellationToken) {
        if (request.IDs.Length > BooleanQuery.MaxClauseCount) {
            return DeleteDocumentsResponse.From(
                Error.Failure(MAX_BOOLEAN_CLAUSE_EXCEEDED_ERROR)
            );
        }

        var clauses = request.IDs
                             .TakeWhile(_ => !cancellationToken.IsCancellationRequested)
                             .Select(CreateBooleanClause)
                             .ToArray();

        if (cancellationToken.IsCancellationRequested) {
            return DeleteDocumentsResponse.From(
                Error.Failure(REQUEST_CANCELLED_ERROR)
            );
        }

        var query = new BooleanQuery();
        ListExtensions.AddRange(query.Clauses, clauses);

        var index = _indexProvider.Get(request.IndexName);

        var delete = index.Delete(query);
        if (delete.Failure) {
            return DeleteDocumentsResponse.From(delete.Errors);
        }

        var save = index.SaveChanges();

        return save.Match(
            onSuccess: _ => DeleteDocumentsResponse.From(request.IDs.Length),
            onFailure: DeleteDocumentsResponse.From
        );

        static BooleanClause CreateBooleanClause(object id) {
            var term = new Term(Constants.DOCUMENT_ID_PROP, id.ToString());
            var termQuery = new TermQuery(term);

            return new BooleanClause(termQuery, Occur.SHOULD);
        }
    }

    /// <inheritdoc />
    public Task<DeleteDocumentsByQueryResponse> DeleteByQueryAsync(DeleteDocumentsByQueryRequest request, CancellationToken cancellationToken) {
        var index = _indexProvider.Get(request.IndexName);

        var count = index.Count(request.Query);
        if (count.Failure) {
            return DeleteDocumentsByQueryResponse.From(count.Errors);
        }

        if (count is { Success: true, Value: 0 }) {
            return DeleteDocumentsByQueryResponse.From(count: 0);
        }
        
        var delete = index.Delete(request.Query);
        if (delete.Failure) {
            return DeleteDocumentsByQueryResponse.From(delete.Errors);
        }

        var save = index.SaveChanges();

        return save.Match(
            onSuccess: _ => DeleteDocumentsByQueryResponse.From(count.Value),
            onFailure: DeleteDocumentsByQueryResponse.From
        );
    }

    /// <inheritdoc />
    public Task<SearchDocumentsResponse<TDocument>> SearchAsync<TDocument>(SearchDocumentsRequest request, CancellationToken cancellationToken)
        where TDocument : class, new() {
        var index = _indexProvider.Get(request.IndexName);
        var count = index.Count(request.Query);

        if (count.Failure) {
            return SearchDocumentsResponse<TDocument>.From(count.Errors);
        }

        if (count is { Success: true, Value: 0 }) {
            return SearchDocumentsResponse<TDocument>.From(documents: [], totalCount: 0);
        }

        if (cancellationToken.IsCancellationRequested) {
            return SearchDocumentsResponse<TDocument>.From(
                Error.Failure(REQUEST_CANCELLED_ERROR)
            );
        }

        var collector = Collectors.CreateTopCollector(
            request.Start,
            request.Limit,
            request.Sort
        );

        var search = index.Search(request.Query, collector);

        return search.Match(
            onSuccess: value => SearchDocumentsResponse<TDocument>.From(
                documents: [.. value.Select(document => _entityDocumentBuilder.Build<TDocument>([.. document]))],
                totalCount: count.Value
            ),
            onFailure: SearchDocumentsResponse<TDocument>.From
        );
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TDocument> StreamAsync<TDocument>(SearchDocumentsRequest request)
        where TDocument : class, new() {
        return new StreamAsyncEnumerable<TDocument>(
            _entityDocumentBuilder,
            _indexProvider.Get(request.IndexName),
            request
        );
    }
}