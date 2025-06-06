using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Nameless;

/// <summary>
///     <see cref="IEnumerable" /> extension methods.
/// </summary>
public static class EnumerableExtensions {
    /// <summary>
    ///     Interact through an instance of <see cref="IEnumerable{T}" />.
    /// </summary>
    /// <typeparam name="T">The enumerable argument type.</typeparam>
    /// <param name="self">An instance of <see cref="IEnumerable{T}" />.</param>
    /// <param name="action">The iterator action.</param>
    public static void Each<T>(this IEnumerable<T> self, Action<T> action) {
        Each(self, (current, _) => action(current));
    }

    /// <summary>
    ///     Interact through an instance of <see cref="IEnumerable{T}" />.
    ///     And pass an index value to the iterator action.
    /// </summary>
    /// <typeparam name="T">The enumerable argument type.</typeparam>
    /// <param name="self">An instance of <see cref="IEnumerable{T}" />.</param>
    /// <param name="action">The iterator action.</param>
    public static void Each<T>(this IEnumerable<T> self, Action<T, int> action) {
        var counter = 0;
        using var enumerator = self.GetEnumerator();
        while (enumerator.MoveNext()) {
            action(enumerator.Current, counter++);
        }
    }

    /// <summary>
    ///     Interact through an instance of <see cref="IEnumerable" />.
    /// </summary>
    /// <param name="self">An instance of <see cref="IEnumerable" />.</param>
    /// <param name="action">The iterator action.</param>
    public static void Each(this IEnumerable self, Action<object?> action) {
        Each(self, (current, _) => action(current));
    }

    /// <summary>
    ///     Interact through an instance of <see cref="IEnumerable" />.
    ///     And pass an index value to the iterator action.
    /// </summary>
    /// <param name="self">An instance of <see cref="IEnumerable" />.</param>
    /// <param name="action">The iterator action.</param>
    public static void Each(this IEnumerable self, Action<object?, int> action) {
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
    ///     Checks if an <see cref="IEnumerable" /> is empty.
    /// </summary>
    /// <param name="self">The <see cref="IEnumerable" /> instance.</param>
    /// <returns><c>true</c>, if is empty, otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this IEnumerable? self) {
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

    /// <summary>
    ///     <strong>(Syntax sugar)</strong> Converts an <see cref="IEnumerable{T}" /> instance into a
    ///     <see cref="IReadOnlyCollection{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <param name="self">The self <see cref="IEnumerable{T}" />.</param>
    /// <returns>An <see cref="IReadOnlyCollection{T}" /> instance.</returns>
    public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> self) {
        return new ReadOnlyCollection<T>(self.ToList());
    }

    /// <summary>
    ///     Selects distinct the self <see cref="IEnumerable{T}" /> by an expression.
    /// </summary>
    /// <typeparam name="TSource">Type of the <see cref="IEnumerable{T}" /></typeparam>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <param name="self">The self <see cref="IEnumerable{T}" />.</param>
    /// <param name="keySelector">The key selector function.</param>
    /// <returns>A filtered collection.</returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector) {
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
    /// <typeparam name="T">The type of the enumerable element.</typeparam>
    /// <param name="self">The enumerable himself.</param>
    /// <returns>An <see cref="IEnumerable" /> of <see cref="Tuple" />.</returns>
    public static IEnumerable<(int Index, T Item)> WithIndex<T>(this IEnumerable<T> self) {
        var counter = 0;
        foreach (var element in self) {
            yield return (counter++, element);
        }
    }
}