namespace Nameless {

    /// <summary>
    /// Extension methods for generic arrays.
    /// </summary>
    public static class ArrayExtension {

        #region Public Static Methods

        /// <summary>
        /// Tries get an array item by its index.
        /// </summary>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <param name="self">The array</param>
        /// <param name="index">The index</param>
        /// <param name="output">The output value for the index</param>
        /// <returns><c>true</c> if the value in the specified index was found; otherwise <c>false</c></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool TryGetByIndex<T>(this T[] self, int index, out T? output) {
            Prevent.Null(self, nameof(self));

            output = default;

            if (self.Length > index) {
                output = self[index];
                return true;
            }

            return false;
        }

        #endregion
    }
}
