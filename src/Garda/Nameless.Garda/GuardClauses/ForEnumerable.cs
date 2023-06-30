using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless {
    public static class ForEnumerable {
        #region Public Static Methods

        [DebuggerStepThrough]
        public static T NullOrEmpty<T>(this IGuardClause _, [NotNull] T input, string name, string? message = null) where T : class, IEnumerable {
            if (input == null) {
                throw new ArgumentNullException(name, message ?? $"Argument {name} is null.");
            }

            // Costs O(1)
            if (input is ICollection collection && collection.Count == 0) {
                throw new ArgumentException(message ?? $"Argument {name} is empty.", name);
            }

            // Costs O(N)
            var enumerator = input.GetEnumerator();
            var canMoveNext = enumerator.MoveNext();
            if (enumerator is IDisposable disposable) {
                disposable.Dispose();
            }
            if (!canMoveNext) {
                throw new ArgumentException(message ?? $"Argument {name} is empty.", name);
            }
            return input;
        }
    }

    #endregion
}