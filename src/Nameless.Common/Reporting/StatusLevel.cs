namespace Nameless.Reporting;

/// <summary>
///     Severity/intent of a <see cref="StatusUpdate"/>. Purely
///     informational - consumers decide how (or whether) to distinguish
///     these visually.
/// </summary>
public enum StatusLevel {
    /// <summary>
    ///     Information
    /// </summary>
    Info,
    /// <summary>
    ///     Warning
    /// </summary>
    Warning,
    /// <summary>
    ///     Error
    /// </summary>
    Error
}