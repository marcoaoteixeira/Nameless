namespace Nameless.Validation;

/// <summary>
///     <see cref="ValidationResult" /> extension methods.
/// </summary>
public static class ValidationResultExtensions {
    /// <param name="self">
    ///     The current <see cref="ValidationResult" />.
    /// </param>
    extension(ValidationResult self) {
        /// <summary>
        ///     Converts the validation result into a dictionary.
        /// </summary>
        /// <returns>
        ///     A dictionary containing all entries from the validation result.
        /// </returns>
        public IDictionary<string, string[]> ToDictionary() {
            if (self.Success) { return new Dictionary<string, string[]>(); }

            return self.Errors
                       .GroupBy(error => error.MemberName)
                       .ToDictionary(
                           group => group.Key,
                           group => group.Select(item => item.Error)
                                         .ToArray());
        }
    }

    /// <param name="self">
    ///     The current <see cref="ValidationResult" />.
    /// </param>
    extension(IEnumerable<ValidationResult> self) {
        /// <summary>
        ///     Aggregates multiple <see cref="ValidationResult" /> instances
        ///     into a single one.
        /// </summary>
        /// <returns>
        ///     A single <see cref="ValidationResult" /> instance containing
        ///     all errors from the provided instances.
        /// </returns>
        public ValidationResult Aggregate() {
            var errors = self.Where(item => !item.Success)
                             .SelectMany(item => item.Errors);

            return ValidationResult.Create([.. errors]);
        }
    }
}