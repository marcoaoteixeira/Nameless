using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.CommandQuery {
    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand {
        #region Private Read-Only Fields

        private readonly IValidator<TCommand>? _validator = null;

        #endregion

        #region Public Properties

        private ILogger? _logger = null;
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

        protected abstract Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);

        #endregion

        #region ICommandHandler<TCommand> Members

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default) {
            if (_validator != null) {
                await _validator.ValidateAsync(command, opts => opts.ThrowOnFailures(), cancellationToken);
            }

            await ExecuteAsync(command, cancellationToken);
        }

        #endregion
    }
}
