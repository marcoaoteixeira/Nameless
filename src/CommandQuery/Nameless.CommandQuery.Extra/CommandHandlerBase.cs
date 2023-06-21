using FluentValidation;
using Nameless.Logging;

namespace Nameless.CommandQuery {

    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand {

        #region Private Fields

        private ILogger? _logger = null;
        private IValidator<TCommand>? _validator = null;

        #endregion

        #region Public Properties

        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Protected Constructors

        protected CommandHandlerBase(IValidator<TCommand>? validator = null) {
            _validator = validator;
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract Task InnerHandleAsync(TCommand command, CancellationToken cancellationToken = default);

        #endregion

        #region ICommandHandler<TCommand> Members

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default) {
            if (_validator != null) {
                await _validator.ValidateAsync(command, opts => opts.ThrowOnFailures(), cancellationToken);
            }

            await InnerHandleAsync(command, cancellationToken);
        }

        #endregion
    }
}
