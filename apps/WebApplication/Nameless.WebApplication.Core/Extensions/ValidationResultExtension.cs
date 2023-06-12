using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nameless.WebApplication {

    public static class ValidationResultExtension {

        #region Public Static Methods

        public static void PushErrorsIntoModelState(this ValidationResult self, ModelStateDictionary modelState) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(modelState, nameof(modelState));

            foreach (var error in self.Errors) {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        #endregion
    }
}
