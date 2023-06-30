using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless {
    public static class ForString {
        #region Public Static Methods

        [DebuggerStepThrough]
        public static string NullOrWhiteSpace(this IGuardClause self, [NotNull] string input, string name, string? message = null) {
            if (input == null) {
                throw new ArgumentNullException(name, message ?? $"Argument {name} is null.");
            }

            if (input.Trim().Length == 0) {
                throw new ArgumentException(message ?? $"Argument {name} is empty or white space.", name);
            }

            return input;
        }

        #endregion
    }
}
