using AutoMapper;
using FluentValidation;
using Nameless.Infrastructure;
using Nameless.Logging;

namespace Nameless.CommandQuery {

    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand {

        #region Private Fields

        private ILogger _logger = default!;

        #endregion

        #region Protected Properties

        protected IMapper Mapper { get; }
        protected IValidator<TCommand>? Validator { get; }
        protected ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Protected Constructors

        protected CommandHandlerBase(IMapper mapper, IValidator<TCommand>? validator = default) {
            Prevent.Null(mapper, nameof(mapper));

            Mapper = mapper;
            Validator = validator;
        }

        #endregion

        #region Protected Abstract Methods

        protected abstract Task<ExecutionResult> InnerHandleAsync(TCommand command, CancellationToken cancellationToken = default);

        #endregion

        #region ICommandHandler<TCommand> Members

        public async Task<ExecutionResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default) {
            if (Validator != default) {
                var result = await Validator.ValidateAsync(command, cancellationToken);
                if (!result.IsValid) {
                    return Mapper.Map<ExecutionResult>(result);
                }
            }

            return await InnerHandleAsync(command, cancellationToken);
        }

        #endregion
    }
}
