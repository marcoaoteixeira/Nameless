using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.FluentValidation.Impl {
    public sealed class ValidatorManager : IValidatorManager {
        #region Private Read-Only Fields

        private readonly IValidator[] _validators;
        private readonly ILogger _logger;
        private readonly Dictionary<Type, IValidator> _cache = [];

        #endregion

        #region Public Constructors

        public ValidatorManager(IValidator[] validators, ILogger logger) {
            _validators = Guard.Against.Null(validators, nameof(validators));
            _logger = logger ?? NullLogger.Instance;
        }

        #endregion

        #region Private Methods

        private bool TryGetValidatorFor<T>([NotNullWhen(returnValue: true)] out IValidator<T>? validator) {
            if (TryGetValidatorFromCache(out validator)) {
                return true;
            }

            if (TryGetValidatorFromValidators(out validator)) {
                _cache[typeof(T)] = validator; // Add to the cache

                return true;
            }

            return false;
        }

        private bool TryGetValidatorFromCache<T>([NotNullWhen(returnValue: true)] out IValidator<T>? validator) {
            validator = null;

            if (_cache.TryGetValue(typeof(T), out var current)) {
                validator = (IValidator<T>)current;

                return true;
            }

            return false;
        }

        private bool TryGetValidatorFromValidators<T>([NotNullWhen(returnValue: true)] out IValidator<T>? validator) {
            validator = null;

            var current = _validators.SingleOrDefault(item => item.CanValidateInstancesOfType(typeof(T)));
            if (current is not null) {
                validator = (IValidator<T>)current;

                return true;
            }

            return false;
        }

        #endregion

        #region IValidatorManager Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default) {
            Guard.Against.Null(instance, nameof(instance));

            if (!TryGetValidatorFor<T>(out var validator)) {
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
