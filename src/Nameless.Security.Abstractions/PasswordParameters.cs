namespace Nameless.Security;

/// <summary>
///     Password parameters.
/// </summary>
public sealed record PasswordParameters {
    public static readonly PasswordParameters Default = new();

    /// <summary>
    ///     Default symbols values.
    /// </summary>
    public const string DefaultSymbols = "*$-+?_&=!%{}/";

    /// <summary>
    ///     Default numeric values.
    /// </summary>
    public const string DefaultNumerics = "0123456789";

    /// <summary>
    ///     Default lower case values.
    /// </summary>
    public const string DefaultLowerCase = "abcdefgijkmnopqrstwxyz";

    /// <summary>
    ///     Default upper case values.
    /// </summary>
    public const string DefaultUpperCase = "ABCDEFGIJKMNOPQRSTWXYZ";

    private string? _lowerCase;
    private string? _numerics;
    private string? _symbols;
    private string? _upperCase;

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
    ///     If not set, will use <see cref="DefaultSymbols" />.
    /// </summary>
    public string Symbols {
        get => _symbols.WithFallback(DefaultSymbols);
        set => _symbols = value;
    }

    /// <summary>
    ///     Gets or sets the numerics to use in the password.
    ///     If not set, will use <see cref="DefaultNumerics" />.
    /// </summary>
    public string Numerics {
        get => _numerics.WithFallback(DefaultNumerics);
        set => _numerics = value;
    }

    /// <summary>
    ///     Gets or sets the lower case chars to use in the password.
    ///     If not set, will use <see cref="DefaultLowerCase" />.
    /// </summary>
    public string LowerCases {
        get => _lowerCase.WithFallback(DefaultLowerCase);
        set => _lowerCase = value;
    }

    /// <summary>
    ///     Gets or sets the upper case chars to use in the password.
    ///     If not set, will use <see cref="DefaultUpperCase" />.
    /// </summary>
    public string UpperCases {
        get => _upperCase.WithFallback(DefaultUpperCase);
        set => _upperCase = value;
    }
}