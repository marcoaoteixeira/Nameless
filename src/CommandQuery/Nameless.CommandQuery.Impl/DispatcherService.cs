using Autofac;

namespace Nameless.CommandQuery {
    public sealed class DispatcherService : IDispatcherService {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public DispatcherService(ILifetimeScope scope) {
            _scope = Prevent.Against.Null(scope, nameof(scope));
        }

        #endregion

        #region IDispatcherService Members

        public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand {
            Prevent.Against.Null(command, nameof(command));

            var service = typeof(ICommandHandler<>).MakeGenericType(typeof(TCommand));
            var implementation = _scope.Resolve(service)
                ?? throw new InvalidOperationException("Command handler not found.");
            var handlerName = nameof(ICommandHandler<TCommand>.HandleAsync);
            var handler = implementation.GetType().GetMethod(handlerName)
                ?? throw new InvalidOperationException($"Cannot find method {handlerName} on handler ${implementation.GetType().Name}");

            var result = handler.Invoke(implementation, parameters: new object[] { command, cancellationToken });

            return (Task)result!;
        }

        public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) {
            Prevent.Against.Null(query, nameof(query));

            var service = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var implementation = _scope.Resolve(service)
                ?? throw new InvalidOperationException("Query handler not found.");
            var handlerName = nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync);
            var handler = implementation.GetType().GetMethod(handlerName)
                ?? throw new InvalidOperationException($"Cannot find method {handlerName} on handler ${implementation.GetType().Name}");

            var result = handler.Invoke(implementation, parameters: new object[] { query, cancellationToken });

            return (Task<TResult>)result!;
        }

        #endregion
    }
}
