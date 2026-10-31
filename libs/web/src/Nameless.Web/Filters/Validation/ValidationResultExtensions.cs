using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nameless.ObjectModel;

namespace Nameless.Web.Filters.Validation;

/// <summary>
///     <see cref="Error"/> extension methods.
/// </summary>
public static class ErrorExtensions {
    extension(IEnumerable<Error> self) {
        /// <summary>
        ///     Converts the collection of <see cref="Error"/> into a
        ///     <see cref="ModelStateDictionary"/> instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="ModelStateDictionary"/> instance.
        /// </returns>
        public ModelStateDictionary ToModelStateDictionary() {
            var result = new ModelStateDictionary();

            foreach (var error in self) {
                result.AddModelError(
                    error.Code ?? string.Empty,
                    error.Message
                );
            }

            return result;
        }
    }
}
