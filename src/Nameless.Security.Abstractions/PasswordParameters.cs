namespace Nameless.Security;

/// <summary>
///     Password parameters.
/// </summary>
public sealed record PasswordParameters {
    /// <summary>
    ///     Default symbols values.
    /// </summary>
    public const string DEFAULT_SYMBOLS = "*$-+?_&=!%{}/";

    /// <summary>
    ///     Default numeric values.
    /// </summary>
    public const string DEFAULT_NUMERICS = "0123456789";

    /// <summary>
    ///     Default lower case values.
    /// </summary>
    public const string DEFAULT_LOWER_CASE = "abcdefgijkmnopqrstwxyz";

    /// <summary>
    ///     Default upper case values.
    /// </summary>
    public const string DEFAULT_UPPER_CASE = "ABCDEFGIJKMNOPQRSTWXYZ";

    /// <summary>
    ///     Gets or sets the minimum length for the password.
    /// </summary>
    /// <remarks>Default is 6</remarks>
    public int MinLength { get; set; } = 6;

    /// <summary>
    ///     Gets or sets the maximum length for the password.
    /// </summary>
    /// <remarks>Default is 12</remarks>
    public int MaxLength { get; set; } = 12;

    /// <summary>
    ///     Gets or sets the symbols to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <see cref="DEFAULT_SYMBOLS" />.
    /// </remarks>
    public string Symbols { get; set; } = DEFAULT_SYMBOLS;

    /// <summary>
    ///     Gets or sets the numerics to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <see cref="DEFAULT_NUMERICS" />.
    /// </remarks>
    public string Numerics { get; set; } = DEFAULT_NUMERICS;

    /// <summary>
    ///     Gets or sets the lower case chars to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <see cref="DEFAULT_LOWER_CASE" />.
    /// </remarks>
    public string LowerCases { get; set; } = DEFAULT_LOWER_CASE;

    /// <summary>
    ///     Gets or sets the upper case chars to use in the password.
    /// </summary>
    /// <remarks>
    ///     Default is <see cref="DEFAULT_UPPER_CASE" />.
    /// </remarks>
    public string UpperCases { get; set; } = DEFAULT_UPPER_CASE;
}