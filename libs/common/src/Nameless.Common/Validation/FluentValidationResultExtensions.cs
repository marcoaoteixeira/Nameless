using Nameless.ObjectModel;

namespace Nameless.Validation;

/// <summary>
///     <see cref="FluentValidation.Results.ValidationResult"/> extension methods.
/// </summary>
public static class FluentValidationResultExtensions {
    /// <param name="self">
    ///     The collection of <see cref="FluentValidation.Results.ValidationResult"/> to transform.
    /// </param>
    extension(IEnumerable<FV_ValidationResult> self) {
        /// <summary>
        ///     Transforms a collection of <see cref="FluentValidation.Results.ValidationResult"/>
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