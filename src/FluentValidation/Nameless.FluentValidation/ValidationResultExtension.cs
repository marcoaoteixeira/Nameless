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
            if (!self.IsValid) {
                foreach (var item in self.Errors) {
                    result.Push(item.ErrorCode, item.ErrorMessage);
                }
            }
            return result;
        }

        public static IDictionary<string, string[]> ToDictionary(this ValidationResult self) {
            var result = new Dictionary<string, string[]>();
            if (!self.IsValid) {
                foreach (var item in self.Errors) {
                    result.Add(item.ErrorCode, new[] { item.ErrorMessage });
                }
            }
            return result;
        }

        #endregion
    }
}
