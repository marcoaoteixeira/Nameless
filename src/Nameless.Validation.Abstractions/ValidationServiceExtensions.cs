namespace Nameless.Validation;

/// <summary>
///     <see cref="IValidationService"/> extension methods.
/// </summary>
public static class ValidationServiceExtensions {
    private static readonly Dictionary<string, object> EmptyContext = [];

    extension(IValidationService self) {
        public Task<ValidationResult> ValidateAsync(object value, CancellationToken cancellationToken) {
            return self.ValidateAsync(value, EmptyContext, cancellationToken);
        }
    }
}
