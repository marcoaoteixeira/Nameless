namespace Nameless.Validation.Abstractions;

public class ValidationException : Exception {
    #region Public Properties

    public ValidationResult Result { get; }

    #endregion

    #region Public Constructors

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

    #endregion
}