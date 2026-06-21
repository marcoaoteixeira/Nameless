namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     Controls how a property is stored and indexed in a Lucene document.
/// </summary>
[Flags]
public enum PropertyOptions {
    /// <summary>
    ///     None
    /// </summary>
    None = 1,

    /// <summary>
    ///     Stores the property as-is.
    /// </summary>
    Store = 2,

    /// <summary>
    ///     Tokenize / Analyze the property. Only applicable to string types.
    /// </summary>
    Analyze = 4,

    /// <summary>
    ///     Sanitize the field, e.g., remove HTML tags.
    ///     Only applicable to string types.
    /// </summary>
    Sanitize = 8
}
