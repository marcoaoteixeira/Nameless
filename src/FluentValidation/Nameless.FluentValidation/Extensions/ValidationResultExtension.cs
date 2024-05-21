using FluentValidation;
using FluentValidation.Results;
using Nameless.ErrorHandling;

namespace Nameless.FluentValidation {
    public static class ValidationResultExtension {
        #region Public Static Methods

        // Syntax sugar
        public static bool Success(this ValidationResult self)
            => self.IsValid;

        // Syntax sugar
        public static bool Failure(this ValidationResult self)
            => !self.IsValid;

        public static ErrorCollection ToErrorCollection(this ValidationResult self)
            => new(ToDictionary(self));

        public static IDictionary<string, string[]> ToDictionary(this ValidationResult self)
            => ToDictionary(self.Errors);

        public static IDictionary<string, string[]> ToDictionary(this ValidationException self)
            => ToDictionary(self.Errors);

        #endregion

        #region Private Static Methods

        private static Dictionary<string, string[]> ToDictionary(IEnumerable<ValidationFailure> failures)
            => failures.ToDictionary<ValidationFailure?, string, string[]>(
                keySelector: failure => failure.PropertyName ?? failure.ErrorCode,
                elementSelector: failure => [failure.ErrorMessage]);

        #endregion
    }
}
