using Nameless.ObjectModel;

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
        public ValidationResult Aggregate() {
            return self.Where(item => !item.IsValid)
                       .SelectMany(item => item.Errors)
                       .Select(error => Error.Validation(
                           error.ErrorMessage,
                           error.PropertyName ?? error.ErrorCode ?? string.Empty)
                       ).ToArray();
        }
    }
}