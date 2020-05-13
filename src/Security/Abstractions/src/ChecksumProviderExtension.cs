using System.IO;

namespace Nameless.Security {

    /// <summary>
    /// Extension methods for <see cref="IChecksumProvider"/>.
    /// </summary>
	public static class ChecksumProviderExtension {

        #region Public Static Methods

        /// <summary>
        /// Generates a checksum for a file.
        /// </summary>
        /// <param name="source">The source <see cref="IChecksumProvider"/>.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="string"/> representation of the checksum.</returns>
        public static string Generate (this IChecksumProvider source, string filePath) {
            if (source == null) { return null; }

            return source.Generate (File.ReadAllBytes (filePath));
        }

        /// <summary>
        /// Validates the checksum of a file.
        /// </summary>
        /// <param name="source">The source <see cref="IChecksumProvider"/>.</param>
        /// <param name="checksum">The checksum to validate.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
        public static bool Validate (this IChecksumProvider source, string checksum, string filePath) {
            if (source == null) { return false; }

            return source.Validate (checksum, File.ReadAllBytes (filePath));
        }

        #endregion
    }
}