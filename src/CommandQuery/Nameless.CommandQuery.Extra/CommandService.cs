using Autofac;

namespace Nameless.CommandQuery {
    public sealed class CommandService : ICommandService {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public CommandService(ILifetimeScope scope) {
            Garda.Prevent.Null(scope, nameof(scope));

            _scope = scope;
        }

        #endregion

        #region ICommandService Members

        public Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default) {
            Garda.Prevent.Null(command, nameof(command));

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _scope.Resolve(handlerType);

            return handler.HandleAsync((dynamic)command, cancellationToken);
        }

        #endregion
    }
}
