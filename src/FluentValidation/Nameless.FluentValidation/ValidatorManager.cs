using Autofac;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.FluentValidation {
    public sealed class ValidatorManager : IValidatorManager {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value;
        }

        #endregion

        #region Public Constructors

        public ValidatorManager(ILifetimeScope scope) {
            _scope = Guard.Against.Null(scope, nameof(scope));
        }

        #endregion

        #region IValidatorManager Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default) {
            Guard.Against.Null(instance, nameof(instance));

            var validator = _scope.ResolveOptional<IValidator<T>>();

            if (validator is null) {
                Logger.LogInformation("Validator for {typeof(T).FullName} not found", typeof(T).FullName);

                return Task.FromResult(new ValidationResult());
            }

            return validator.ValidateAsync(
                instance,
                opts => { if (throwOnError) { opts.ThrowOnFailures(); } },
                cancellationToken
            );
        }

        #endregion
    }
}
