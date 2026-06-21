namespace Nameless;

/// <summary>
///     <see cref="byte" /> array extension methods.
/// </summary>
public static class ByteArrayExtensions {
    /// <param name="self">
    ///     The source array of <see cref="byte" />.
    /// </param>
    extension(byte[] self) {
        /// <summary>
        ///     Transforms an array of <see cref="byte" />s into a hexadecimal
        ///     <see cref="string" /> representation.
        /// </summary>
        /// <returns>
        ///     A hexadecimal <see cref="string" /> representation of the
        ///     <see cref="byte" /> array.
        /// </returns>
        public string ToHexString() {
            return Convert.ToHexString(self).Replace(oldValue: "-", string.Empty);
        }

        /// <summary>
        ///     <strong>(Syntax sugar)</strong> Convert a byte array to a
        ///     Base64 string.
        /// </summary>
        /// <param name="options">
        ///     The convert options.
        /// </param>
        /// <returns>
        ///     A base64 string representation for the current array.
        /// </returns>
        public string ToBase64String(Base64FormattingOptions options = Base64FormattingOptions.None) {
            return Convert.ToBase64String(self, options);
        }
    }
}