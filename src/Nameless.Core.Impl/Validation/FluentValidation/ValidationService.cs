using FluentValidation;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Default implementation of <see cref="IValidationService"/> that uses
///     FluentValidation for validation.
/// </summary>
public class ValidationService : IValidationService {
    private readonly IValidator[] _validators;

    /// <summary>
    ///     Initializes a new instance of <see cref="ValidationService" />.
    /// </summary>
    /// <param name="validators"> A collection of <see cref="IValidator" />.</param>
    public ValidationService(IEnumerable<IValidator> validators) {
        // we might need to iterate over it multiple times,
        // so we convert it to an array
        _validators = [.. validators];
    }

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(object value, IDictionary<string, object> context, CancellationToken cancellationToken) {
        Throws.When.Null(value);

        var type = value.GetType();
        var validationContext = CreateValidationContext();
        var validations = _validators.Where(validator => validator.CanValidateInstancesOfType(type))
                                     .Select(validator => validator.ValidateAsync(validationContext, cancellationToken));
        
        var results = await Task.WhenAll(validations).SkipContextSync();

        return results.Aggregate();

        ValidationContext<object> CreateValidationContext() {
            var result = new ValidationContext<object>(value);
            
            foreach (var key in context.Keys) {
                result.RootContextData[key] = context[key];
            }

            return result;
        }
    }
}