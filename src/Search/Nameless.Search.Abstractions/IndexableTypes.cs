namespace Nameless.Search;

/// <summary>
/// Enumerator for indexable types.
/// </summary>
public enum IndexableType {
    /// <summary>
    /// String/Text values.
    /// </summary>
    Text,
    /// <summary>
    /// Integer numbers.
    /// </summary>
    Integer,
    /// <summary>
    /// Date/Time values.
    /// </summary>
    DateTime,
    /// <summary>
    /// True/False values
    /// </summary>
    Boolean,
    /// <summary>
    /// Float point or decimal numbers.
    /// </summary>
    Number
}