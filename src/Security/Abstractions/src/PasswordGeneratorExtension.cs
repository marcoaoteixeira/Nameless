namespace Nameless.Security {

    /// <summary>
    /// Extension methods for <see cref="IPasswordGenerator"/>.
    /// </summary>
	public static class PasswordGeneratorExtension {
        #region Public Static Read-Only Fields

        /// <summary>
        /// Gets the default minimun password length.
        /// </summary>
        public static readonly int DefaultMinPasswordLength = 8;

        /// <summary>
        /// Gets the default maximun password length.
        /// </summary>
		public static readonly int DefaultMaxPasswordLength = 10;

        #endregion Public Static Read-Only Fields

        #region	Public Static Methods

        /// <summary>
        /// Generates a password.
        /// </summary>
        /// <param name="source">The source <see cref="IPasswordGenerator"/>.</param>
        /// <returns>The <see cref="string"/> representation of the generated password.</returns>
        public static string Generate (this IPasswordGenerator source) {
            if (source == null) { return null; }

            return source.Generate (DefaultMinPasswordLength, DefaultMaxPasswordLength);
        }

        /// <summary>
        /// Generates a password.
        /// </summary>
        /// <param name="source">The source <see cref="IPasswordGenerator"/>.</param>
        /// <param name="length">The password length.</param>
        /// <returns>The <see cref="string"/> representation of the generated password.</returns>
		public static string Generate (this IPasswordGenerator source, int length) {
            if (source == null) { return null; }

            return source.Generate (length, length);
        }

        #endregion
    }
}