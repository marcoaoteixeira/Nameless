namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="char"/>.
    /// </summary>
    public static class CharExtension {

        #region Public Static Methods

        /// <summary>
        /// Determines whether a character is a letter. Syntax sugar.
        /// </summary>
        /// <param name="self">The <see cref="char"/>.</param>
        /// <returns><c>true</c> if is a letter, otherwise, <c>false</c>.</returns>
        public static bool IsLetter(this char self) => char.IsLetter(self);

        /// <summary>
        /// Determines whether a character is whitespace. Syntax sugar.
        /// </summary>
        /// <param name="self">The <see cref="char"/>.</param>
        /// <returns><c>true</c> if is a white space, otherwise, <c>false</c>.</returns>
        public static bool IsBlank(this char self) => char.IsWhiteSpace(self);

        #endregion
    }
}