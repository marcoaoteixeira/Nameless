namespace Nameless;

/// <summary>
///     <see cref="char" /> extension methods.
/// </summary>
public static class CharExtensions {
    /// <param name="self">
    ///     The <see cref="char" />.
    /// </param>
    extension(char self) {
        /// <summary>
        ///     <strong>(Syntax sugar)</strong> Indicates whether the specified
        ///     Unicode character is categorized as a Unicode digit.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if is a digit;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsDigit() {
            return char.IsDigit(self);
        }

        /// <summary>
        ///     <strong>(Syntax sugar)</strong> Indicates whether the specified
        ///     Unicode character is categorized as a Unicode letter.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if is a letter;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsLetter() {
            return char.IsLetter(self);
        }

        /// <summary>
        ///     <strong>(Syntax sugar)</strong> Indicates whether the specified
        ///     Unicode character is categorized as white space.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if is a white space;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsWhiteSpace() {
            return char.IsWhiteSpace(self);
        }

        /// <summary>
        ///     <strong>(Syntax sugar)</strong> Indicates whether the specified
        ///     Unicode character is categorized as an uppercase letter.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if is an uppercase letter;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsUpper() {
            return char.IsUpper(self);
        }

        /// <summary>
        ///     <strong>(Syntax sugar)</strong> Indicates whether the specified
        ///     Unicode character is categorized as a lowercase letter.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if is a lowercase letter;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsLower() {
            return char.IsLower(self);
        }
    }
}