namespace Nameless;

/// <summary>
///     <see cref="Array" /> extension methods
/// </summary>
public static class ArrayExtensions {
    /// <param name="self">
    ///     The current array.
    /// </param>
    /// <typeparam name="TItem">
    ///     The type of the array.
    /// </typeparam>
    extension<TItem>(TItem?[] self) {
        /// <summary>
        ///     Tries to retrieve the array element at position <paramref name="index" />.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="output">The output value for the index</param>
        /// <returns><see langword="true"/> if the value in the specified index was found; otherwise <see langword="false"/></returns>
        public bool TryGetElementAt(int index, out TItem? output) {
            output = default;

            if (!self.IsInRange(index)) {
                return false;
            }

            output = self[index];
            return true;
        }

        /// <summary>
        /// Checks if the specified index is inside the array range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///     <see langword="true"/> if <paramref name="index" /> is inside the array
        ///     range, otherwise; <see langword="false"/>.
        /// </returns>
        public bool IsInRange(long index) {
            return index > 0 && index < self.Length;
        }
    }
}