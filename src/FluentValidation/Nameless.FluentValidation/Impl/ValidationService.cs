using System.Collections.Concurrent;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.FluentValidation.Impl {
    public sealed class ValidationService : IValidationService {
        #region Private Read-Only Fields

        private readonly ConcurrentDictionary<Type, IValidator?> _cache = [];
        private readonly IValidator[] _validators;
        private readonly ILogger _logger;
        
        #endregion

        #region Public Constructors

        public ValidationService(IValidator[] validators)
            : this(validators, NullLogger<ValidationService>.Instance) { }

        public ValidationService(IValidator[] validators, ILogger<ValidationService> logger) {
            _validators = Guard.Against.Null(validators, nameof(validators));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Private Methods

        private IValidator? FetchValidator(Type instanceType)
            => _validators.SingleOrDefault(item => item.CanValidateInstancesOfType(instanceType));

        #endregion

        #region IValidationService Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default) {
            Guard.Against.Null(instance, nameof(instance));

            var validator = _cache.GetOrAdd(typeof(T), FetchValidator);
            if (validator is IValidator<T> instanceValidator) {
                return instanceValidator.ValidateAsync(instance, cancellationToken);
            }

            _logger.LogInformation(message: "Validator for {Validator} not found",
                                   args: typeof(T).FullName);

            return Task.FromResult(new ValidationResult());
        }

        #endregion
    }
}
