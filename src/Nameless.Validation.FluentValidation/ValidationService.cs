using FluentValidation;

namespace Nameless.Validation.FluentValidation;

/// <summary>
/// Default implementation of <see cref="IValidationService"/> that uses FluentValidation for validation.
/// </summary>
public sealed class ValidationService : IValidationService {
    private readonly IValidator[] _validators;

    /// <summary>
    ///     Initializes a new instance of <see cref="ValidationService" />.
    /// </summary>
    /// <param name="validators"> A collection of <see cref="IValidator" />.</param>
    public ValidationService(IEnumerable<IValidator> validators) {
        // we might need to iterate over it multiple times,
        // so we convert it to an array
        _validators = validators.ToArray();
    }

    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(object value, CancellationToken cancellationToken) {
        return ValidateAsync(value, new DataContext(), cancellationToken);
    }

    public async Task<ValidationResult> ValidateAsync(object value, DataContext dataContext, CancellationToken cancellationToken) {
        var results = new List<global::FluentValidation.Results.ValidationResult>();
        var availableValidators = _validators.Where(validator => validator.CanValidateInstancesOfType(value.GetType()));
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