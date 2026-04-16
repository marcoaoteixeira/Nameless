namespace Nameless.Security.Password;

/// <summary>
///     Password parameters.
/// </summary>
public record Arguments {
    /// <summary>
    ///     Gets or sets the minimum length for the password.
    /// </summary>
    /// <remarks>Default is 6</remarks>
    public int MinLength { get; init; } = 6;

    /// <summary>
    ///     Gets or sets the maximum length for the password.
    /// </summary>
    /// <remarks>Default is 12</remarks>
    public int MaxLength { get; init; } = 12;

    /// <summary>
    ///     Gets or sets the symbols to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <c>*$-+?_&=!%{}/</c>.
    /// </remarks>
    public string Symbols { get; init; } = "*$-+?_&=!%{}/";

    /// <summary>
    ///     Gets or sets the numerics to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <c>0123456789</c>.
    /// </remarks>
    public string Numerics { get; init; } = "0123456789";

    /// <summary>
    ///     Gets or sets the lower case chars to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <c>abcdefgijkmnopqrstwxyz</c>.
    /// </remarks>
    public string LowerCases { get; init; } = "abcdefgijkmnopqrstwxyz";

    /// <summary>
    ///     Gets or sets the upper case chars to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <c>ABCDEFGIJKMNOPQRSTWXYZ</c>
    /// </remarks>
    public string UpperCases { get; init; } = "ABCDEFGIJKMNOPQRSTWXYZ";
}