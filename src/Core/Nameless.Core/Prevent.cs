using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless {
    public static class Prevent {
        #region Public Static Methods

        /// <summary>
        /// Makes sure that the <paramref name="argumentValue"/> is not <c>null</c>.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void Null([NotNull] object? argumentValue, string? argumentName) {
            if (argumentValue == null) {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Makes sure that the <paramref name="argumentValue"/> is not <c>null</c>.
        /// </summary>
        /// <param name="argumentValue">The argument value</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void Default<T>([NotNull] T argumentValue, string? argumentName) where T : struct {
            if (default(T).Equals(argumentValue)) {
                throw new ArgumentException("Argument value cannot be null.", argumentName);
            }
        }

        /// <summary>
        /// Makes sure that the <paramref name="argumentValue"/> is not <c>null</c>,
        /// empty or white spaces.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="argumentValue"/> is empty or white spaces.
        /// </exception>
        [DebuggerStepThrough]
        public static void NullOrWhiteSpaces([NotNull] string? argumentValue, string? argumentName) {
            Null(argumentValue, argumentName);

            if (argumentValue.Trim().Length == 0) {
                throw new ArgumentException("Argument value cannot be empty or white spaces.", argumentName);
            }
        }

        /// <summary>
        /// Makes sure that the <paramref name="argumentValue"/> is not
        /// <c>null</c> or empty.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="argumentValue"/> is empty.
        /// </exception>
		[DebuggerStepThrough]
        public static void NullOrEmpty([NotNull] IEnumerable? argumentValue, string? argumentName) {
            Null(argumentValue, argumentName);

            // Costs O(1)
            if (argumentValue is ICollection collection && collection.Count == 0) {
                throw new ArgumentException("Argument value cannot be empty.", argumentName);
            }

            // Costs O(N)
            var enumerator = argumentValue.GetEnumerator();
            var canMoveNext = enumerator.MoveNext();
            if (enumerator is IDisposable disposable) {
                disposable.Dispose();
            }
            if (!canMoveNext) {
                throw new ArgumentException("Argument value cannot be empty.", argumentName);
            }
        }

        #endregion
    }
}
