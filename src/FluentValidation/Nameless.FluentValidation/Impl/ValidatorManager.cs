using System.Collections.Concurrent;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.FluentValidation.Impl {
    public sealed class ValidatorManager : IValidatorManager {
        #region Private Read-Only Fields

        private readonly ConcurrentDictionary<Type, IValidator?> Cache = [];

        private readonly IValidator[] _validators;
        private readonly ILogger _logger;
        
        #endregion

        #region Public Constructors

        public ValidatorManager(IValidator[] validators)
            : this(validators, NullLogger.Instance) { }

        public ValidatorManager(IValidator[] validators, ILogger logger) {
            _validators = Guard.Against.Null(validators, nameof(validators));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region IValidatorManager Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default) {
            Guard.Against.Null(instance, nameof(instance));

            var validator = Cache.GetOrAdd(typeof(T), key => {
                var current = _validators
                    .SingleOrDefault(item
                        => item.CanValidateInstancesOfType(key)
                    );

                return current is not null
                    ? current
                    : null;
            }) as IValidator<T>;

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
