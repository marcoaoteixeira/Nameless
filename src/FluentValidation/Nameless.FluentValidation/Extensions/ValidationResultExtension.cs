using FluentValidation;
using FluentValidation.Results;
using Nameless.ErrorHandling;
using Nameless.Services;
using ValidationResult = FluentValidation.Results.ValidationResult;

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

        #region Internal Static Methods

        internal static Services.ValidationResult Convert(this ValidationResult self) {
            if (self.IsValid) { return Services.ValidationResult.Empty; }

            var entries = self.Errors
                              .Select(error => new ValidationEntry(error.PropertyName ?? error.ErrorCode, error.ErrorMessage))
                              .ToArray();

            return new Services.ValidationResult(entries);
        }

        #endregion

        #region Private Static Methods

        private static Dictionary<string, string[]> ToDictionary(IEnumerable<ValidationFailure> failures)
            => failures.ToDictionary(
                keySelector: failure => failure.PropertyName ?? failure.ErrorCode,
                elementSelector: failure => new[] { failure.ErrorMessage }
            );

        #endregion
    }
}
