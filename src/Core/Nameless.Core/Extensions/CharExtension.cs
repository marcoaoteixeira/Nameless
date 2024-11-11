namespace Nameless;

/// <summary>
/// <see cref="char"/> extension methods.
/// </summary>
public static class CharExtension {
    /// <summary>
    /// <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as a Unicode digit.
    /// </summary>
    /// <param name="self">The <see cref="char"/>.</param>
    /// <returns><c>true</c> if is a digit; otherwise, <c>false</c>.</returns>
    public static bool IsDigit(this char self)
        => char.IsDigit(self);

    /// <summary>
    /// <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as a Unicode letter.
    /// </summary>
    /// <param name="self">The <see cref="char"/>.</param>
    /// <returns><c>true</c> if is a letter; otherwise, <c>false</c>.</returns>
    public static bool IsLetter(this char self)
        => char.IsLetter(self);

    /// <summary>
    /// <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as white space.
    /// </summary>
    /// <param name="self">The <see cref="char"/>.</param>
    /// <returns><c>true</c> if is a white space; otherwise, <c>false</c>.</returns>
    public static bool IsWhiteSpace(this char self)
        => char.IsWhiteSpace(self);

    /// <summary>
    /// <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as an uppercase letter.
    /// </summary>
    /// <param name="self">The <see cref="char"/>.</param>
    /// <returns><c>true</c> if is an uppercase letter; otherwise, <c>false</c>.</returns>
    public static bool IsUpper(this char self)
        => char.IsUpper(self);

    /// <summary>
    /// <strong>(Syntax sugar)</strong> Indicates whether the specified Unicode character is categorized as a lowercase letter.
    /// </summary>
    /// <param name="self">The <see cref="char"/>.</param>
    /// <returns><c>true</c> if is a lowercase letter; otherwise, <c>false</c>.</returns>
    public static bool IsLower(this char self)
        => char.IsLower(self);
}