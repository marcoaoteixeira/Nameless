using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless {
    public static class ForString {
        #region Public Static Methods

        [DebuggerStepThrough]
        public static string NullOrEmpty(this Prevent _, [NotNull] string input, string name, string? message = null) {
            if (input == null) {
                throw new ArgumentNullException(name, message ?? $"Argument {name} cannot be null.");
            }

            if (input.Length == 0) {
                throw new ArgumentException(message ?? $"Argument {name} cannot be empty.", name);
            }

            return input;
        }

        [DebuggerStepThrough]
        public static string NullOrWhiteSpace(this Prevent _, [NotNull] string input, string name, string? message = null) {
            if (input == null) {
                throw new ArgumentNullException(name, message ?? $"Argument {name} cannot be null.");
            }

            if (input.Trim().Length == 0) {
                throw new ArgumentException(message ?? $"Argument {name} cannot be empty or white space.", name);
            }

            return input;
        }

        #endregion
    }
}
