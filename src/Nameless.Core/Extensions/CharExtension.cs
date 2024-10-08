﻿namespace Nameless;

/// <summary>
/// <see cref="char"/> extension methods.
/// </summary>
public static class CharExtension {
    /// <summary>
    /// <strong>(Syntax sugar)</strong> Determines whether a character is a letter.
    /// </summary>
    /// <param name="self">The <see cref="char"/>.</param>
    /// <returns><c>true</c> if is a letter, otherwise, <c>false</c>.</returns>
    public static bool IsLetter(this char self)
        => char.IsLetter(self);

    /// <summary>
    /// <strong>(Syntax sugar)</strong> Determines whether a character is whitespace.
    /// </summary>
    /// <param name="self">The <see cref="char"/>.</param>
    /// <returns><c>true</c> if is a white space, otherwise, <c>false</c>.</returns>
    public static bool IsWhiteSpace(this char self)
        => char.IsWhiteSpace(self);
}