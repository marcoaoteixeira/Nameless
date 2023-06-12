using Autofac;
using Nameless.Infrastructure;

namespace Nameless.CommandQuery {
    public sealed class CommandDispatcher : ICommandDispatcher {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public CommandDispatcher(ILifetimeScope scope) {
            Prevent.Null(scope, nameof(scope));

            _scope = scope;
        }

        #endregion

        #region ICommandDispatcher Members

        public Task<ExecutionResult> ExecuteAsync(ICommand command, CancellationToken cancellationToken = default) {
            Prevent.Null(command, nameof(command));

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _scope.Resolve(handlerType);

            return (Task<ExecutionResult>)handler.HandleAsync((dynamic)command, cancellationToken);
        }

        #endregion
    }
}
