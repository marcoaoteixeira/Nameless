using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nameless.Infrastructure;
using Nameless.WebApplication.Domain;

namespace Nameless.WebApplication {

    public static class ExecutionResultExtension {

        #region Public Static Methods

        // Syntax sugar
        public static Task<ExecutionResult> AsTask(this ExecutionResult self) => Task.FromResult(self);

        public static void PushErrorsIntoModelState(this ExecutionResult self, ModelStateDictionary modelState) {
            if (self == default) { return; }

            foreach (var error in self.Errors) {
                modelState.AddModelError(error.Property, error.Message);
            }
        }

        public static ErrorOutput ToErrorOutput(this ExecutionResult self) {
            if (self == default || self.Success) { return ErrorOutput.Empty; }

            var dictionary = self.Errors
                .ToDictionary(
                    keySelector: _ => _.Property,
                    elementSelector: _ => self.Errors
                        .Where(error => error.Property == _.Property)
                        .Select(error => error.Message)
                        .ToArray()
                );

            return new(dictionary);
        }

        #endregion
    }
}
