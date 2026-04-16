using Microsoft.AspNetCore.Mvc;
using Nameless.ObjectModel;

namespace Nameless.Web;

public static class ErrorExtensions {
    extension(IEnumerable<Error> self) {
        public ProblemDetails ToProblemDetails() {
            return new ProblemDetails {
                Detail = string.Join("; ", self.Select(error => error.Message))
            };
        }

        public Dictionary<string, string[]> ToDictionary() {
            return self.GroupBy(error => error.Code).ToDictionary(
                keySelector: group => group.Key ?? string.Empty,
                elementSelector: group => group.Select(item => item.Message).ToArray()
            );
        }
    }
}
