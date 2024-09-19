namespace Nameless.Validation.Abstractions;

public interface IValidationService {
    Task<ValidationResult> ValidateAsync<TValue>(TValue value, bool throwOnFailure, CancellationToken cancellationToken)
        where TValue : class;
}