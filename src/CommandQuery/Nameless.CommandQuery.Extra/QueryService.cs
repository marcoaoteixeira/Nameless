using Autofac;

namespace Nameless.CommandQuery {
    public sealed class QueryService : IQueryService {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public QueryService(ILifetimeScope scope) {
            Garda.Prevent.Null(scope, nameof(scope));

            _scope = scope;
        }

        #endregion

        #region IQueryService Members

        public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) {
            Garda.Prevent.Null(query, nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _scope.Resolve(handlerType);

            return handler.HandleAsync((dynamic)query, cancellationToken);
        }

        #endregion
    }
}
