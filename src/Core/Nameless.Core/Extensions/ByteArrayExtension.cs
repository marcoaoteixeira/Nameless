namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="byte"/> arrays.
    /// </summary>
    public static class ByteArrayExtension {

        #region Public Static Methods

        /// <summary>
        /// Transforms an array of <see cref="byte"/>s into a hexadecimal <see cref="string"/> representation.
        /// </summary>
        /// <param name="self">The source array of <see cref="byte"/>.</param>
        /// <returns>A hexadecimal <see cref="string"/> representation of the <see cref="byte"/> array.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string ToHexString(this byte[] self) {
            Prevent.Null(self, nameof(self));

            return BitConverter.ToString(self).Replace("-", string.Empty);
        }

        #endregion
    }
}