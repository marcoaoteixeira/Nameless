using Autofac;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.FluentValidation.Impl {
    public sealed class ValidatorService : IValidatorService {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;
        private readonly ILogger _logger;

        #endregion

        #region Public Constructors

        public ValidatorService(ILifetimeScope scope) {
            _scope = Guard.Against.Null(scope, nameof(scope));
            _logger = CreateLogger(scope);
        }

        #endregion

        #region Private Static Methods

        private static ILogger CreateLogger(ILifetimeScope scope) {
            var loggerFactory = scope.ResolveOptional<ILoggerFactory>();
            return loggerFactory is not null
                ? loggerFactory.CreateLogger<ValidatorService>()
                : NullLogger<ValidatorService>.Instance;
        }

        #endregion

        #region IValidatorManager Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default) {
            Guard.Against.Null(instance, nameof(instance));

            var validator = _scope.ResolveOptional<IValidator<T>>();

            if (validator is null) {
                _logger
                    .LogInformation(
                        message: "Validator for {FullName} not found",
                        args: typeof(T).FullName
                    );

                return Task.FromResult(new ValidationResult());
            }

            return validator.ValidateAsync(instance, cancellationToken);
        }

        #endregion
    }
}
