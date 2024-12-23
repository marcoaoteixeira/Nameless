namespace Nameless.Search;

/// <summary>
/// Enumerator for document index options.
/// </summary>
[Flags]
public enum FieldOptions {
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Store (no tokenize)
    /// </summary>
    Store = 1,
    /// <summary>
    /// Analyze (tokenize)
    /// </summary>
    Analyze = 2,
    /// <summary>
    /// Sanitize the field
    /// </summary>
    Sanitize = 4
}