namespace Nameless.Validation.Abstractions;

public static class ValidationResultExtension {
    #region Public Static Methods

    public static IDictionary<string, string[]> ToDictionary(this ValidationResult self) {
        if (self.Succeeded) { return new Dictionary<string, string[]>(); }

        return self.Errors
                   .GroupBy(error => error.Code)
                   .ToDictionary(keySelector: group => group.Key,
                                 elementSelector: group => group.Select(item => item.Message)
                                                                .ToArray());
    }

    #endregion
}