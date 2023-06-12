using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nameless.WebApplication {
    
    public static class IdentityResultExtension {

        #region Public Static Methods

        public static void PushErrorsIntoModelState(this IdentityResult self, ModelStateDictionary modelState) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(modelState, nameof(modelState));

            foreach (var error in self.Errors) {
                modelState.AddModelError(error.Code, error.Description);
            }
        }

        #endregion
    }
}
