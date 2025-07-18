﻿namespace Nameless;

/// <summary>
///     <see cref="char" /> extension methods.
/// </summary>
public static class CharExtensions {
    /// <summary>
    ///     <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as a Unicode
    ///     digit.
    /// </summary>
    /// <param name="self">The <see cref="char" />.</param>
    /// <returns><see langword="true"/> if is a digit; otherwise, <see langword="false"/>.</returns>
    public static bool IsDigit(this char self) {
        return char.IsDigit(self);
    }

    /// <summary>
    ///     <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as a Unicode
    ///     letter.
    /// </summary>
    /// <param name="self">The <see cref="char" />.</param>
    /// <returns><see langword="true"/> if is a letter; otherwise, <see langword="false"/>.</returns>
    public static bool IsLetter(this char self) {
        return char.IsLetter(self);
    }

    /// <summary>
    ///     <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as white space.
    /// </summary>
    /// <param name="self">The <see cref="char" />.</param>
    /// <returns><see langword="true"/> if is a white space; otherwise, <see langword="false"/>.</returns>
    public static bool IsWhiteSpace(this char self) {
        return char.IsWhiteSpace(self);
    }

    /// <summary>
    ///     <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as an uppercase
    ///     letter.
    /// </summary>
    /// <param name="self">The <see cref="char" />.</param>
    /// <returns><see langword="true"/> if is an uppercase letter; otherwise, <see langword="false"/>.</returns>
    public static bool IsUpper(this char self) {
        return char.IsUpper(self);
    }

    /// <summary>
    ///     <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as a lowercase
    ///     letter.
    /// </summary>
    /// <param name="self">The <see cref="char" />.</param>
    /// <returns><see langword="true"/> if is a lowercase letter; otherwise, <see langword="false"/>.</returns>
    public static bool IsLower(this char self) {
        return char.IsLower(self);
    }
}