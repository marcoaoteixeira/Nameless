namespace Nameless.Validation;

/// <summary>
///     <see cref="ValidationResult" /> extension methods.
/// </summary>
public static class ValidationResultExtensions {
    /// <summary>
    ///     Converts the validation result into a dictionary.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="ValidationResult" />.
    /// </param>
    /// <returns>
    ///     A dictionary containing all entries from the validation result.
    /// </returns>
    public static IDictionary<string, string[]> ToDictionary(this ValidationResult self) {
        if (self.Succeeded) { return new Dictionary<string, string[]>(); }

        return self.Errors
                   .GroupBy(error => error.MemberName)
                   .ToDictionary(
                       group => group.Key,
                       group => group.Select(item => item.Error)
                                     .ToArray());
    }

    /// <summary>
    ///     Aggregates multiple <see cref="ValidationResult" /> instances
    ///     into a single one.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="ValidationResult" />.
    /// </param>
    /// <returns>
    ///     A single <see cref="ValidationResult" /> instance containing
    ///     all errors from the provided instances.
    /// </returns>
    public static ValidationResult Aggregate(this IEnumerable<ValidationResult> self) {
        var array = Guard.Against
                         .Null(self)
                         .Where(item => !item.Succeeded)
                         .SelectMany(item => item.Errors)
                         .ToArray();

        return array.Length == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(array);
    }
}