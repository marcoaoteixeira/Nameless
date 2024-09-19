namespace Nameless.Validation.Abstractions;

public class ValidationException : Exception {
    public ValidationResult Result { get; }

    public ValidationException(ValidationResult result) {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public ValidationException(ValidationResult result, string message)
        : base(message) {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public ValidationException(ValidationResult result, string message, Exception inner)
        : base(message, inner) {
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }
}