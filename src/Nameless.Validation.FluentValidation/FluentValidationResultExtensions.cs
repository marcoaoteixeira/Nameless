namespace Nameless.Validation.FluentValidation;

/// <summary>
///     <see cref="FluentValidationResult"/> extension methods.
/// </summary>
public static class FluentValidationResultExtensions {
    /// <param name="self">
    ///     The collection of <see cref="FluentValidationResult"/> to transform.
    /// </param>
    extension(IEnumerable<FluentValidationResult> self) {
        /// <summary>
        ///     Transforms a collection of <see cref="FluentValidationResult"/>
        ///     into a single <see cref="ValidationResult"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="ValidationResult"/> representing the combined results.
        /// </returns>
        public ValidationResult ToValidationResult() {
            var array = Guard.Against
                             .Null(self)
                             .Where(item => !item.IsValid)
                             .SelectMany(item => item.Errors)
                             .Select(error => new ValidationError(
                                 error.ErrorMessage,
                                 error.ErrorCode,
                                 error.PropertyName))
                             .ToArray();

            return array.Length == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(array);
        }
    }
}