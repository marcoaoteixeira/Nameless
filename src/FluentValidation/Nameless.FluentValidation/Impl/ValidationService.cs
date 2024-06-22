using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Services;

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

        public async Task<ValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default) {
            Guard.Against.Null(instance, nameof(instance));

            var validator = _cache.GetOrAdd(instance.GetType(), FetchValidator);
            if (validator is not null) {
                var context = new ValidationContext<object>(instance);
                var result = await validator.ValidateAsync(context, cancellationToken);

                return result.Convert();
            }

            _logger.LogInformation(message: "Validator for {InstanceTypeFullName} not found",
                                   args: instance.GetType().FullName);

            return ValidationResult.Empty;
        }

        #endregion
    }
}
