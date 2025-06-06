namespace Nameless.Validation;

/// <summary>
///     <see cref="ValidationResult" /> extension methods.
/// </summary>
public static class ValidationResultExtensions {
    /// <summary>
    ///     Converts the validation result into a dictionary.
    /// </summary>
    /// <param name="self">The <see cref="ValidationResult" /> current instance.</param>
    /// <returns>
    ///     A dictionary containing all entries from the validation result.
    /// </returns>
    public static IDictionary<string, string[]> ToDictionary(this ValidationResult self) {
        if (self.Succeeded) { return new Dictionary<string, string[]>(); }

        return self.Errors
                   .GroupBy(error => error.MemberName)
                   .ToDictionary(
                        keySelector: group => group.Key,
                        elementSelector: group => group.Select(item => item.Message)
                                                       .ToArray());
    }
}