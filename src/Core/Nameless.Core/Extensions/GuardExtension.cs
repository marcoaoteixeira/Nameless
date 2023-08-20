using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Nameless {
    public static class GuardExtension {
        #region Public Static Methods

        [DebuggerStepThrough]
        public static T Null<T>(this Guard _, [NotNull] T? input, string name, string? message = null)
            => input ?? throw new ArgumentNullException(name, message ?? $"Argument {name} cannot be null.");

        [DebuggerStepThrough]
        public static string NullOrEmpty(this Guard _, [NotNull] string? input, string name, string? message = null) {
            if (input is null) {
                throw new ArgumentNullException(name, message ?? $"Argument {name} cannot be null.");
            }

            if (input.Length == 0) {
                throw new ArgumentException(message ?? $"Argument {name} cannot be empty.", name);
            }

            return input;
        }

        [DebuggerStepThrough]
        public static string NullOrWhiteSpace(this Guard _, [NotNull] string? input, string name, string? message = null) {
            if (input is null) {
                throw new ArgumentNullException(name, message ?? $"Argument {name} cannot be null.");
            }

            if (input.Trim().Length == 0) {
                throw new ArgumentException(message ?? $"Argument {name} cannot be empty or white space.", name);
            }

            return input;
        }

        [DebuggerStepThrough]
        public static T NullOrEmpty<T>(this Guard _, [NotNull] T? input, string name, string? message = null) where T : class, IEnumerable {
            if (input is null) {
                throw new ArgumentNullException(name, message ?? $"Argument {name} cannot be null.");
            }

            // Costs O(1)
            if (input is ICollection collection && collection.Count == 0) {
                throw new ArgumentException(message ?? $"Argument {name} cannot be empty.", name);
            }

            // Costs O(N)
            var enumerator = input.GetEnumerator();
            var canMoveNext = enumerator.MoveNext();
            if (enumerator is IDisposable disposable) {
                disposable.Dispose();
            }
            if (!canMoveNext) {
                throw new ArgumentException(message ?? $"Argument {name} cannot be empty.", name);
            }
            return input;
        }

        [DebuggerStepThrough]
        public static string NoMatchingPattern(this Guard _, string input, string name, string pattern, string? message = null) {
            var match = Regex.Match(input, pattern);
            if (!match.Success || match.Value != input) {
                throw new ArgumentException(message ?? $"Argument {name} does not match pattern {pattern}", name);
            }
            return input;
        }

        #endregion
    }
}