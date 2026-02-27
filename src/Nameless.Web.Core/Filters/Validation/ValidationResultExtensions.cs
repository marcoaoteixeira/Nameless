using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nameless.ObjectModel;
using Nameless.Validation;

namespace Nameless.Web.Filters.Validation;

public static class ValidationResultExtensions {
    extension(ValidationResult self) {
        public IEnumerable<Error> ToErrors() {
            return self.Errors.Select(error
                => Error.Validation(error.Error, error.MemberName)
            );
        }
    }
}

public static class ErrorExtensions {
    extension(IEnumerable<Error> self) {
        public ModelStateDictionary ToModelStateDictionary() {
            var result = new ModelStateDictionary();

            foreach (var error in self) {
                result.AddModelError(error.Code ?? string.Empty, error.Message);
            }

            return result;
        }
    }
}