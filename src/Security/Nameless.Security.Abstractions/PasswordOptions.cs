namespace Nameless.Security;

/// <summary>
/// Password options.
/// </summary>
public sealed record PasswordOptions {
    /// <summary>
    /// Default symbols values.
    /// </summary>
    public const string DEFAULT_SYMBOLS = "*$-+?_&=!%{}/";

    /// <summary>
    /// Default numeric values.
    /// </summary>
    public const string DEFAULT_NUMERICS = "0123456789";

    /// <summary>
    /// Default lower case values.
    /// </summary>
    public const string DEFAULT_LOWER_CASE = "abcdefgijkmnopqrstwxyz";
    
    /// <summary>
    /// Default upper case values.
    /// </summary>
    public const string DEFAULT_UPPER_CASE = "ABCDEFGIJKMNOPQRSTWXYZ";

    private string? _symbols;
    private string? _numerics;
    private string? _lowerCase;
    private string? _upperCase;

    /// <summary>
    /// Gets or sets the minimum length for the password.
    /// </summary>
    /// <remarks>Default is 6</remarks>
    public int MinLength { get; set; } = 6;
    /// <summary>
    /// Gets or sets the maximum length for the password.
    /// </summary>
    /// <remarks>Default is 12</remarks>
    public int MaxLength { get; set; } = 12;

    /// <summary>
    /// Gets or sets if whether will use special chars.
    /// </summary>
    /// <remarks>Default are <see cref="DEFAULT_SYMBOLS"/></remarks>
    public string Symbols {
        get => _symbols.WithFallback(DEFAULT_SYMBOLS);
        set => _symbols = value;
    }
    /// <summary>
    /// Gets or sets if whether will use numbers.
    /// </summary>
    /// <remarks>Default are <c>0123456789</c></remarks>
    public string Numerics {
        get => _numerics.WithFallback(DEFAULT_NUMERICS);
        set => _numerics = value;
    }
    /// <summary>
    /// Gets or sets if whether will use lower case chars.
    /// </summary>
    /// <remarks>Default are <c>abcdefgijkmnopqrstwxyz</c></remarks>
    public string LowerCases {
        get => _lowerCase.WithFallback(DEFAULT_LOWER_CASE);
        set => _lowerCase = value;
    }
    /// <summary>
    /// Gets or sets if whether will use upper case chars.
    /// </summary>
    /// <remarks>Default are <c>ABCDEFGIJKMNOPQRSTWXYZ</c></remarks>
    public string UpperCases {
        get => _upperCase.WithFallback(DEFAULT_UPPER_CASE);
        set => _upperCase = value;
    }
}