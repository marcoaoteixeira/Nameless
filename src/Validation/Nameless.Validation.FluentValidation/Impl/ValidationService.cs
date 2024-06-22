using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Validation.Abstractions;
using ValidationException = Nameless.Validation.Abstractions.ValidationException;

namespace Nameless.Validation.FluentValidation.Impl {
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
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Private Methods

        private IValidator? FetchValidator(Type instanceType)
            => _validators.SingleOrDefault(item => item.CanValidateInstancesOfType(instanceType));

        #endregion

        #region IValidationService Members

        public async Task<ValidationResult> ValidateAsync<TValue>(TValue value, bool throwOnFailure = false, CancellationToken cancellationToken = default)
            where TValue : class {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }

            var cacheEntry = _cache.GetOrAdd(typeof(TValue), FetchValidator);
            if (cacheEntry is IValidator<TValue> validator) {
                var response = await validator.ValidateAsync(value, cancellationToken);
                var result = Helper.Map(response);

                if (!result.Succeeded && throwOnFailure) {
                    throw new ValidationException(result);
                }

                return result;
            }

            _logger.LogInformation(message: "Validator for {Value} not found",
                                   args: typeof(TValue).FullName);

            return ValidationResult.Empty;
        }

        #endregion
    }
}
