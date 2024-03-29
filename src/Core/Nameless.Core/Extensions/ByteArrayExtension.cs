﻿namespace Nameless {
    /// <summary>
    /// <see cref="byte"/> array extension methods.
    /// </summary>
    public static class ByteArrayExtension {
        #region Public Static Methods

        /// <summary>
        /// Transforms an array of <see cref="byte"/>s into a hexadecimal <see cref="string"/> representation.
        /// </summary>
        /// <param name="self">The source array of <see cref="byte"/>.</param>
        /// <returns>A hexadecimal <see cref="string"/> representation of the <see cref="byte"/> array.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string ToHexString(this byte[] self)
            => BitConverter.ToString(self ?? []).Replace("-", string.Empty);

        public static string ToBase64String(this byte[] self, Base64FormattingOptions options = Base64FormattingOptions.None)
            => Convert.ToBase64String(self, options);

        #endregion
    }
}