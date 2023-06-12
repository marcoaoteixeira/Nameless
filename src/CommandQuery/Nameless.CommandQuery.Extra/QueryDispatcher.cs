using Autofac;

namespace Nameless.CommandQuery {
    public sealed class QueryDispatcher : IQueryDispatcher {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public QueryDispatcher(ILifetimeScope scope) {
            Prevent.Null(scope, nameof(scope));

            _scope = scope;
        }

        #endregion

        #region IQueryDispatcher Members

        public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) {
            Prevent.Null(query, nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _scope.Resolve(handlerType);

            return (Task<TResult>)handler.HandleAsync((dynamic)query, cancellationToken);
        }

        #endregion
    }
}
