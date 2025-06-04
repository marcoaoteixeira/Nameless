using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Nameless.Validation.FluentValidation;

/// <summary>
/// Default implementation of <see cref="IValidationService"/> that uses FluentValidation for validation.
/// </summary>
public sealed class ValidationService : IValidationService {
    private readonly IValidator[] _validators;
    private readonly ILogger<ValidationService> _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="ValidationService" />.
    /// </summary>
    /// <param name="validators"> A collection of <see cref="IValidator" />.</param>
    /// <param name="logger">The logger.</param>
    public ValidationService(IEnumerable<IValidator> validators, ILogger<ValidationService> logger) {
        // we might need to iterate over it multiple times,
        // so we convert it to an array
        _validators = validators.ToArray();
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(object value, CancellationToken cancellationToken) {
        return ValidateAsync(value, new DataContext(), cancellationToken);
    }

    public async Task<ValidationResult> ValidateAsync(object value, DataContext dataContext, CancellationToken cancellationToken) {
        var availableValidators = _validators.Where(validator => validator.CanValidateInstancesOfType(value.GetType()))
                                             .ToArray();

        if (availableValidators.Length == 0) {
            _logger.MissingValidatorForType(value);

            return ValidationResult.Success();
        }

        var results = new List<global::FluentValidation.Results.ValidationResult>();
        foreach (var validator in availableValidators) {
            var validationContext = CreateValidationContext(value, dataContext);
            var result = await validator.ValidateAsync(validationContext, cancellationToken);

            results.Add(result);
        }

        return results.ToValidationResult();

        static ValidationContext<object> CreateValidationContext(object value, DataContext dataContext) {
            var context = new ValidationContext<object>(value);
            foreach (var key in dataContext.Keys) {
                context.RootContextData[key] = dataContext[key];
            }
            return context;
        }
    }
}