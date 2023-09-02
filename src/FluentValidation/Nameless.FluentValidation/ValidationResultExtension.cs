using FluentValidation;
using FluentValidation.Results;
using Nameless.ErrorHandling;

namespace Nameless.FluentValidation {
    public static class ValidationResultExtension {
        #region Public Static Methods

        // Syntax sugar
        public static bool Successful(this ValidationResult self)
            => self.IsValid;

        // Syntax sugar
        public static bool Failure(this ValidationResult self)
            => !self.IsValid;

        public static ErrorCollection ToErrorCollection(this ValidationResult self) {
            var result = new ErrorCollection();
            foreach (var item in self.Errors) {
                result.Push(item.ErrorCode, item.ErrorMessage);
            }
            return result;
        }

        public static IDictionary<string, string[]> ToDictionary(this ValidationResult self)
            => ToDictionary(self.Errors);

        public static IDictionary<string, string[]> ToDictionary(this ValidationException self)
            => ToDictionary(self.Errors);

        #endregion

        #region Private Static Methods

        private static IDictionary<string, string[]> ToDictionary(IEnumerable<ValidationFailure> failures) {
            var result = new Dictionary<string, string[]>();
            foreach (var failure in failures) {
                result.Add(failure.ErrorCode, new[] { failure.ErrorMessage });
            }
            return result;
        }

        #endregion
    }
}
