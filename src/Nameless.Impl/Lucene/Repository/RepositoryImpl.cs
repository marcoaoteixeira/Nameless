using Nameless.Lucene.Repository.Infrastructure;
using Nameless.Lucene.Repository.Mappings;
using Nameless.Lucene.Repository.Requests;
using Nameless.Lucene.Repository.Responses;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Repository;

public class RepositoryImpl : IRepository {
    private const string REQUEST_CANCELLED_ERROR = "Requested task was cancelled.";

    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly IMapper _mapper;
    private readonly IIndexProvider _indexProvider;

    public RepositoryImpl(IAnalyzerProvider analyzerProvider, IIndexProvider indexProvider, IMapper mapper) {
        _analyzerProvider = analyzerProvider;
        _indexProvider = indexProvider;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public Task<InsertEntitiesResponse> InsertAsync<TEntity>(InsertEntitiesRequest<TEntity> request, CancellationToken cancellationToken) where TEntity : class {
        var collection = request.Entities.CreateDocumentCollection(_mapper);
        var index = _indexProvider.Get(request.IndexName);

        var insert = index.Insert(collection);
        if (insert.Failure) {
            return InsertEntitiesResponse.From(insert.Errors);
        }

        if (cancellationToken.IsCancellationRequested) {
            var rollback = index.Rollback();

            return InsertEntitiesResponse.From([
                Error.Failure(REQUEST_CANCELLED_ERROR),
                ..rollback.Failure ? rollback.Errors : []
            ]);
        }

        var save = index.SaveChanges();
        return save.Match(
            onSuccess: _ => InsertEntitiesResponse.From(collection.Count),
            onFailure: InsertEntitiesResponse.From
        );
    }

    /// <inheritdoc />
    public Task<DeleteEntitiesResponse> DeleteAsync<TEntity>(DeleteEntitiesRequest<TEntity> request, CancellationToken cancellationToken)
        where TEntity : class {
        var query = request.Entities.CreateDeleteQuery(_mapper);
        if (query.Failure) {
            return DeleteEntitiesResponse.From(query.Errors);
        }

        var index = _indexProvider.Get(request.IndexName);

        var count = index.Count(query.Value);
        if (count.Failure) {
            return DeleteEntitiesResponse.From(count.Errors);
        }

        var delete = index.Delete(query.Value);
        if (delete.Failure) {
            return DeleteEntitiesResponse.From(delete.Errors);
        }

        if (cancellationToken.IsCancellationRequested) {
            var rollback = index.Rollback();

            return DeleteEntitiesResponse.From([
                Error.Failure(REQUEST_CANCELLED_ERROR),
                ..rollback.Failure ? rollback.Errors : []
            ]);
        }

        var save = index.SaveChanges();
        return save.Match(
            onSuccess: _ => DeleteEntitiesResponse.From(count.Value),
            onFailure: DeleteEntitiesResponse.From
        );
    }

    /// <inheritdoc />
    public Task<DeleteEntitiesByQueryResponse> DeleteByQueryAsync(DeleteEntitiesByQueryRequest request, CancellationToken cancellationToken) {
        var builder = CreateQueryBuilder(request.IndexName);

        request.Query(builder);

        var query = builder.Build().Query;
        var index = _indexProvider.Get(request.IndexName);

        var count = index.Count(query);
        if (count.Failure) {
            return DeleteEntitiesByQueryResponse.From(count.Errors);
        }

        var delete = index.Delete(query);
        if (delete.Failure) {
            return DeleteEntitiesByQueryResponse.From(delete.Errors);
        }

        if (cancellationToken.IsCancellationRequested) {
            var rollback = index.Rollback();

            return DeleteEntitiesByQueryResponse.From([
                Error.Failure(REQUEST_CANCELLED_ERROR),
                ..rollback.Failure ? rollback.Errors : []
            ]);
        }

        var save = index.SaveChanges();
        return save.Match(
            onSuccess: _ => DeleteEntitiesByQueryResponse.From(count.Value),
            onFailure: DeleteEntitiesByQueryResponse.From
        );
    }

    /// <inheritdoc />
    public Task<UpdateEntitiesResponse> UpdateAsync<TEntity>(UpdateEntitiesRequest<TEntity> request, CancellationToken cancellationToken) where TEntity : class {
        var query = request.Entities.CreateDeleteQuery(_mapper);
        if (query.Failure) {
            return UpdateEntitiesResponse.From(query.Errors);
        }

        var index = _indexProvider.Get(request.IndexName);

        var delete = index.Delete(query.Value);
        if (delete.Failure) {
            return UpdateEntitiesResponse.From(delete.Errors);
        }

        var documents = request.Entities.CreateDocumentCollection(_mapper);
        var insert = index.Insert(documents);
        if (insert.Failure) {
            return UpdateEntitiesResponse.From(insert.Errors);
        }

        if (cancellationToken.IsCancellationRequested) {
            var rollback = index.Rollback();

            return UpdateEntitiesResponse.From([
                Error.Failure(REQUEST_CANCELLED_ERROR),
                ..rollback.Failure ? rollback.Errors : []
            ]);
        }

        var save = index.SaveChanges();
        return save.Match(
            onSuccess: _ => UpdateEntitiesResponse.From(request.Entities.Length),
            onFailure: UpdateEntitiesResponse.From
        );
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> SearchAsync<TEntity>(SearchEntitiesRequest request) where TEntity : class, new() {
        var builder = CreateQueryBuilder(request.IndexName);

        request.Query(builder);

        var queryDefinition = builder.Build();
        var index = _indexProvider.Get(request.IndexName);
        var enumerable = index.Search(
            queryDefinition.Query,
            queryDefinition.Sort,
            queryDefinition.Limit
        );

        return new AsyncEnumerableWrapper<TEntity>(enumerable, _mapper);
    }

    private QueryBuilder CreateQueryBuilder(string? indexName) {
        return QueryBuilder.Create(
            analyzer: _analyzerProvider.GetAnalyzer(indexName)
        );
    }
}
