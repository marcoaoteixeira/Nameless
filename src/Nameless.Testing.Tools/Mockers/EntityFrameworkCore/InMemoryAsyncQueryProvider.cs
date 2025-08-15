using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Nameless.Testing.Tools.Mockers.EntityFrameworkCore;

public class InMemoryAsyncQueryProvider<TEntity> : IAsyncQueryProvider {
    private readonly IQueryProvider _queryProvider;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="InMemoryAsyncQueryProvider{TEntity}"/> class.
    /// </summary>
    /// <param name="queryProvider">
    ///     The query provider.
    /// </param>
    public InMemoryAsyncQueryProvider(IQueryProvider queryProvider) {
        _queryProvider = queryProvider;
    }

    public IQueryable CreateQuery(Expression expression) {
        return new InMemoryAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
        return new InMemoryAsyncEnumerable<TElement>(expression);
    }

    public object? Execute(Expression expression) {
        return _queryProvider.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression) {
        return _queryProvider.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default) {
        var result = Execute(expression);

        var resultType = typeof(TResult).GetGenericArguments().FirstOrDefault();
        if (resultType is null) {
            throw new InvalidOperationException("Return type do not provide generic argument.");
        }

        var handler = typeof(Task).GetMethod(nameof(Task.FromResult))?.MakeGenericMethod(resultType);
        if (handler is null) {
            throw new InvalidOperationException($"Could not find handler for {nameof(Task.FromResult)}.");
        }

        var handlerResult = handler.Invoke(null, [result]);
        if (handlerResult is null) {
            throw new InvalidOperationException($"{nameof(Task.FromResult)} handler didn't return valid result.");
        }

        return (TResult)handlerResult;
    }
}
