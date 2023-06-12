using Autofac;
using Nameless.Infrastructure;

namespace Nameless.CommandQuery
{

    public sealed class DispatcherService : IDispatcherService {

        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public DispatcherService(ILifetimeScope scope) {
            Prevent.Null(scope, nameof(scope));

            _scope = scope;
        }

        #endregion

        #region IDispatcherService Members

        public Task<ExecutionResult> ExecuteAsync(ICommand command, CancellationToken cancellationToken = default) {
            Prevent.Null(command, nameof(command));

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _scope.Resolve(handlerType);

            return (Task<ExecutionResult>)handler.HandleAsync((dynamic)command, cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) {
            Prevent.Null(query, nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _scope.Resolve(handlerType);

            return (Task<TResult>)handler.HandleAsync((dynamic)query, cancellationToken);
        }

        #endregion
    }
}