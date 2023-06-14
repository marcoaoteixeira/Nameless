using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtension {

        #region Public Static Methods

        /// <summary>
        /// Interact through an instance of <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The enumerable argument type.</typeparam>
        /// <param name="self">An instance of <see cref="IEnumerable{T}"/>.</param>
        /// <param name="action">The iterator action.</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="self"/> or <paramref name="action"/> were <c>null</c>.
        /// </exception>
        public static void Each<T>(this IEnumerable<T> self, Action<T> action) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(action, nameof(action));

            Each(self, (current, _) => action(current));
        }

        /// <summary>
        /// Interact through an instance of <see cref="IEnumerable{T}"/>.
        /// And pass an index value to the iterator action.
        /// </summary>
        /// <typeparam name="T">The enumerable argument type.</typeparam>
        /// <param name="self">An instance of <see cref="IEnumerable{T}"/>.</param>
        /// <param name="action">The iterator action.</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="self"/> or <paramref name="action"/> were <c>null</c>.
        /// </exception>
        public static void Each<T>(this IEnumerable<T> self, Action<T, int> action) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(action, nameof(action));

            var counter = 0;

            using var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext()) {
                action(enumerator.Current, counter++);
            }
        }

        /// <summary>
        /// Interact through an instance of <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="self">An instance of <see cref="IEnumerable"/>.</param>
        /// <param name="action">The iterator action.</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="self"/> or <paramref name="action"/> were <c>null</c>.
        /// </exception>
        public static void Each(this IEnumerable self, Action<object> action) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(action, nameof(action));

            Each(self, (current, _) => action(current));
        }

        /// <summary>
        /// Interact through an instance of <see cref="IEnumerable"/>.
        /// And pass an index value to the iterator action.
        /// </summary>
        /// <param name="self">An instance of <see cref="IEnumerable"/>.</param>
        /// <param name="action">The iterator action.</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="self"/> or <paramref name="action"/> were <c>null</c>.
        /// </exception>
        public static void Each(this IEnumerable self, Action<object, int> action) {
            Prevent.Null(self, nameof(self));
            Prevent.Null(action, nameof(action));

            if (self == default) { return; }

            var counter = 0;
            var enumerator = self.GetEnumerator();

            while (enumerator.MoveNext()) {
                action(enumerator.Current, counter++);
            }

            if (enumerator is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Checks if an <see cref="IEnumerable"/> is empty.
        /// </summary>
        /// <param name="self">The <see cref="IEnumerable"/> instance.</param>
        /// <returns><c>true</c>, if is empty, otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool IsNullOrEmpty([NotNullWhen(false)] this IEnumerable? self) {
            if (self == default) { return true; }

            // Costs O(1)
            if (self is ICollection collection) { return collection.Count == 0; }

            // Costs O(N)
            var enumerator = self.GetEnumerator();
            var canMoveNext = enumerator.MoveNext();

            if (enumerator is IDisposable disposable) {
                disposable.Dispose();
            }

            return !canMoveNext;
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> instance into a <see cref="IReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="self">The self <see cref="IEnumerable{T}"/>.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> instance.</returns>
        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> self) => new ReadOnlyCollection<T>((self ?? Enumerable.Empty<T>()).ToList());

        /// <summary>
        /// Selects distinct the self <see cref="IEnumerable{T}"/> by an expression.
        /// </summary>
        /// <typeparam name="TSource">Type of the <see cref="IEnumerable{T}"/></typeparam>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="self">The self <see cref="IEnumerable{T}"/>.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <returns>A filtered collection.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector) {
            Prevent.Null(self, nameof(self));

            var seenKeys = new HashSet<TKey>();
            foreach (var element in self) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }

        #endregion
    }
}