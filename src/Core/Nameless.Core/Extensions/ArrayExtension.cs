namespace Nameless {
    /// <summary>
    /// <see cref="Array"/> extension methods
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
        public static bool TryElementAt<T>(this T[] self, int index, out T? output) {
            output = default;

            if (self is null) { return false; }

            if (self.Length > index) {
                output = self[index];
                return true;
            }

            return false;
        }

        #endregion
    }
}
