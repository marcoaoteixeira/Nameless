using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Nameless.Validation.FluentValidation.Internals;

namespace Nameless.Validation.FluentValidation;

public sealed class ValidationService : IValidationService {
    private readonly ConcurrentDictionary<Type, IValidator?> _cache = [];
    private readonly IValidator[] _validators;
    private readonly ILogger _logger;

    public ValidationService(IEnumerable<IValidator> validators, ILogger<ValidationService> logger) {
        _validators = Prevent.Argument
                           .Null(validators, nameof(validators))
                           .ToArray();
        _logger = Prevent.Argument.Null(logger);
    }

    public async Task<ValidationResult> ValidateAsync<TValue>(TValue value, bool throwOnFailure = false, CancellationToken cancellationToken = default)
        where TValue : class {
        Prevent.Argument.Null(value);

        var cacheEntry = _cache.GetOrAdd(typeof(TValue), FetchValidator);
        if (cacheEntry is IValidator<TValue> validator) {
            var response = await validator.ValidateAsync(value, cancellationToken);
            var result = Mapper.Map(response);

            if (!result.Succeeded && throwOnFailure) {
                throw new ValidationException(result);
            }

            return result;
        }

        // Validator not found, log it.
        _logger.ValidatorNotFoundFor<TValue>();

        return ValidationResult.Empty;
    }
    
    private IValidator? FetchValidator(Type instanceType)
        => _validators.SingleOrDefault(item => item.CanValidateInstancesOfType(instanceType));
}