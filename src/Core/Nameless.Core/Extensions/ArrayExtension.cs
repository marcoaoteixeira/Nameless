namespace Nameless;

/// <summary>
/// <see cref="Array"/> extension methods
/// </summary>
public static class ArrayExtension {
    /// <summary>
    /// Tries to retrieve the array element at position <paramref name="index"/>.
    /// </summary>
    /// <typeparam name="T">Type of the array</typeparam>
    /// <param name="self">The array</param>
    /// <param name="index">The index</param>
    /// <param name="output">The output value for the index</param>
    /// <returns><c>true</c> if the value in the specified index was found; otherwise <c>false</c></returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>. It doesn't mean its items can't be null, it means the array object.
    /// </exception>
    public static bool TryGetElementAt<T>(this T?[] self, int index, out T? output) {
        output = default;

        Prevent.Argument.Null(self, nameof(self));

        if (!IsInRange(self, index)) {
            return false;
        }

        output = self[index];

        return true;
    }

    /// <summary>
    /// Checks if the <paramref name="index"/> is in the current array range.
    /// </summary>
    /// <typeparam name="T">Type of the array.</typeparam>
    /// <param name="self">The current array.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="index"/> is inside the array
    /// range, otherwise; <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>. It doesn't mean its items can't be null, it means the array object.
    /// </exception>
    public static bool IsInRange<T>(this T?[] self, long index)
        => index > 0 && index < Prevent.Argument.Null(self, nameof(self)).Length;
}