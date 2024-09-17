using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nameless.ErrorHandling;

namespace Nameless.Web;

public static class ModelStateDictionaryExtension {
    public static ErrorCollection ToErrorCollection(this ModelStateDictionary self) {
        Prevent.Argument.Null(self);

        var result = new ErrorCollection();
        foreach (var (key, value) in self) {
            var messageProblems = value.Errors
                                       .Where(error => !string.IsNullOrWhiteSpace(error.ErrorMessage))
                                       .Select(error => error.ErrorMessage);

            var exceptionProblems = value.Errors
                                         .SelectMany(error => GetProblemsFromException(error.Exception).ToArray());

            result.Add(key, messageProblems.Concat(exceptionProblems).ToArray());
        }
        return result;
    }

    private static IEnumerable<string> GetProblemsFromException(Exception? ex) {
        if (ex is null) { yield break; }

        yield return $"[{ex.GetType().Name}] {ex.Message}";

        var stackTrace = ex.StackTrace?.Split(Environment.NewLine) ?? [];
        foreach (var item in stackTrace) {
            yield return item;
        }
    }
}