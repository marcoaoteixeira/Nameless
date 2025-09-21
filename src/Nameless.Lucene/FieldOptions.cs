namespace Nameless.Lucene;

/// <summary>
///     Enumerator for field options.
/// </summary>
[Flags]
public enum FieldOptions {
    /// <summary>
    ///     None
    /// </summary>
    None = 1,

    /// <summary>
    ///     Stores the field as-is.
    /// </summary>
    Store = 2,

    /// <summary>
    ///     Tokenize / Analyze the field.
    ///     Only applicable to string fields.
    /// </summary>
    Analyze = 4,

    /// <summary>
    ///     Sanitize the field, e.g., remove HTML tags.
    ///     Only applicable to string fields.
    /// </summary>
    Sanitize = 8
}