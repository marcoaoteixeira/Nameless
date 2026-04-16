#pragma warning disable S3267

using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Nameless;

/// <summary>
///     <see cref="IEnumerable" /> extension methods.
/// </summary>
public static class EnumerableExtensions {
    /// <param name="self">An instance of <see cref="IEnumerable{T}" />.</param>
    /// <typeparam name="T">The enumerable argument type.</typeparam>
    extension<T>(IEnumerable<T> self) {
        /// <summary>
        ///     Interact through an instance of <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <param name="action">The iterator action.</param>
        public void Each(Action<T> action) {
            self.Each((current, _) => action(current));
        }

        /// <summary>
        ///     Interact through an instance of <see cref="IEnumerable{T}" />.
        ///     And pass an index value to the iterator action.
        /// </summary>
        /// <param name="action">The iterator action.</param>
        public void Each(Action<T, int> action) {
            var counter = 0;
            using var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext()) {
                action(enumerator.Current, counter++);
            }
        }
    }

    /// <param name="self">An instance of <see cref="IEnumerable" />.</param>
    extension(IEnumerable self) {
        /// <summary>
        ///     Interact through an instance of <see cref="IEnumerable" />.
        /// </summary>
        /// <param name="action">The iterator action.</param>
        public void Each(Action<object?> action) {
            self.Each((current, _) => action(current));
        }

        /// <summary>
        ///     Interact through an instance of <see cref="IEnumerable" />.
        ///     And pass an index value to the iterator action.
        /// </summary>
        /// <param name="action">The iterator action.</param>
        public void Each(Action<object?, int> action) {
            var counter = 0;
            var enumerator = self.GetEnumerator();

            while (enumerator.MoveNext()) {
                action(enumerator.Current, counter++);
            }

            if (enumerator is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     Checks if an <see cref="IEnumerable" /> is empty.
    /// </summary>
    /// <param name="self">The <see cref="IEnumerable" /> instance.</param>
    /// <returns><see langword="true"/>, if is empty, otherwise, <see langword="false"/>.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(returnValue: false)] this IEnumerable? self) {
        switch (self) {
            case null:
                return true;

            // Costs O(1)
            case ICollection collection:
                return collection.Count == 0;
        }

        // Costs O(N)
        var enumerator = self.GetEnumerator();
        var canMoveNext = enumerator.MoveNext();

        if (enumerator is IDisposable disposable) {
            disposable.Dispose();
        }

        return !canMoveNext;
    }

    /// <param name="self">The self <see cref="IEnumerable{T}" />.</param>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    extension<T>(IEnumerable<T> self) {
        /// <summary>
        ///     Selects distinct the self <see cref="IEnumerable{T}" /> by an expression.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="keySelector">The key selector function.</param>
        /// <returns>A filtered collection.</returns>
        public IEnumerable<T> DistinctBy<TKey>(Func<T, TKey> keySelector) {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in self) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }

        /// <summary>
        ///     Transforms an <see cref="IEnumerable{T}" /> into an <see cref="IEnumerable" /> of <see cref="Tuple" /> that
        ///     will consist of an index (<see cref="int" />) and the current element <c>T</c>.
        ///     Note: This action will trigger the enumerator, recommendation is to use it at the end of your query.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> of <see cref="Tuple" />.</returns>
        public IEnumerable<(int Index, T Item)> WithIndex() {
            var counter = 0;
            foreach (var element in self) {
                yield return (counter++, element);
            }
        }
    }
}