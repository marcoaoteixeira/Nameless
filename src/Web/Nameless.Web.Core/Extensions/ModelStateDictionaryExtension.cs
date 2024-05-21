using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nameless.ErrorHandling;

namespace Nameless.Web {
    public static class ModelStateDictionaryExtension {
        #region Public Static Methods

        public static ErrorCollection ToErrorCollection(this ModelStateDictionary self) {
            var result = new ErrorCollection();
            foreach (var (key, value) in self) {
                var messageProblems = value.Errors
                                           .Where(error => !string.IsNullOrWhiteSpace(error.ErrorMessage))
                                           .Select(error => error.ErrorMessage);

                var exceptionProblems = value.Errors
                                             .SelectMany(error => GetProblemsFromException(error.Exception).ToArray());

                result.Push(key, messageProblems.Concat(exceptionProblems).ToArray());
            }
            return result;
        }

        #endregion

        #region Private Static Methods

        private static IEnumerable<string> GetProblemsFromException(Exception? ex) {
            if (ex is null) { yield break; }

            yield return $"[{ex.GetType().Name}] {ex.Message}";

            var stackTrace = ex.StackTrace?.Split(Environment.NewLine) ?? [];
            foreach (var item in stackTrace) {
                yield return item;
            }
        }

        #endregion
    }
}
