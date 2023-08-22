using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nameless.ErrorHandling;

namespace Nameless.Web {
    public static class ModelStateDictionaryExtension {
        #region Public Static Methods

        public static ErrorCollection ToErrorCollection(this ModelStateDictionary self) {
            var result = new ErrorCollection();
            foreach (var kvp in self) {
                var code = kvp.Key;

                var messageProblems = kvp.Value.Errors
                    .Where(_ => !string.IsNullOrWhiteSpace(_.ErrorMessage))
                    .Select(_ => _.ErrorMessage);

                var exceptionProblems = kvp.Value.Errors
                    .SelectMany(_ => GetProblemsFromException(_.Exception).ToArray());

                result.Push(code, messageProblems.Concat(exceptionProblems).ToArray());
            }
            return result;
        }

        #endregion

        #region Private Static Methods

        private static IEnumerable<string> GetProblemsFromException(Exception? ex) {
            if (ex is null) { yield break; }

            yield return $"[{ex.GetType().Name}] {ex.Message}";

            var stackTrace = ex.StackTrace?.Split(Environment.NewLine) ?? Array.Empty<string>();
            foreach (var item in stackTrace) {
                yield return item;
            }
        }

        #endregion
    }
}
