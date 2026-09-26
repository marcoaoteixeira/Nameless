namespace Nameless.ObjectModel;

/// <summary>
///     Represents the type of error that occurred.
/// </summary>
public enum ErrorType {
    /// <summary>
    ///     Indicates a validation error, such as invalid input or data format.
    /// </summary>
    Validation,

    /// <summary>
    ///     Indicates a missing resource or required value.
    /// </summary>
    Missing,

    /// <summary>
    ///     Indicates a conflict, such as a duplicate entry or state violation.
    /// </summary>
    Conflict,

    /// <summary>
    ///     Indicates a general failure not covered by other error types.
    /// </summary>
    Failure,

    /// <summary>
    ///     Indicates a forbidden action due to insufficient permissions.
    /// </summary>
    Forbidden,

    /// <summary>
    ///     Indicates an unauthorized action due to lack of authentication.
    /// </summary>
    Unauthorized
}