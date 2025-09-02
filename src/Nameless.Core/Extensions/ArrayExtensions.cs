﻿namespace Nameless;

/// <summary>
///     <see cref="Array" /> extension methods
/// </summary>
public static class ArrayExtensions {
    /// <summary>
    ///     Tries to retrieve the array element at position <paramref name="index" />.
    /// </summary>
    /// <typeparam name="T">Type of the array</typeparam>
    /// <param name="self">The array</param>
    /// <param name="index">The index</param>
    /// <param name="output">The output value for the index</param>
    /// <returns><see langword="true"/> if the value in the specified index was found; otherwise <see langword="false"/></returns>
    public static bool TryGetElementAt<T>(this T?[] self, int index, out T? output) {
        output = default;

        if (!IsInRange(self, index)) {
            return false;
        }

        output = self[index];
        return true;

    }

    /// <summary>
    /// Checks if the specified index is inside the array range.
    /// </summary>
    /// <typeparam name="T">Type of the array.</typeparam>
    /// <param name="self">The current array.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="index" /> is inside the array
    ///     range, otherwise; <see langword="false"/>.
    /// </returns>
    public static bool IsInRange<T>(this T?[] self, long index) {
        return index > 0 && index < self.Length;
    }
}