namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="char"/>.
    /// </summary>
    public static class CharExtension {

        #region Public Static Methods

        /// <summary>
        /// Whether the char is a letter between A and Z or not.
        /// </summary>
        /// <param name="self">The source <see cref="char"/>.</param>
        /// <returns><c>true</c> if is a letter, otherwise, <c>false</c>.</returns>
        public static bool IsLetter (this char self) {
            return ('A' <= self && self <= 'Z') || ('a' <= self && self <= 'z');
        }

        /// <summary>
        /// Whether the char is a space or not
        /// </summary>
        /// <param name="self">The source <see cref="char"/>.</param>
        /// <returns><c>true</c> if is a space, otherwise, <c>false</c>.</returns>
        public static bool IsSpace (this char self) {
            return (self == '\r' || self == '\n' || self == '\t' || self == '\f' || self == ' ');
        }

        #endregion Public Static Methods
    }
}