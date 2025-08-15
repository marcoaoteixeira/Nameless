namespace Nameless.Results;

/// <summary>
///     Types of errors.
/// </summary>
public enum ErrorType {
    /// <summary>
    ///     Represents a validation error.
    /// </summary>
    Validation,

    /// <summary>
    ///     Represents a missing error, indicating that a required resource
    ///     or information is not available. 
    /// </summary>
    Missing,

    /// <summary>
    ///     Represents a conflict error, indicating that a request could not
    ///     be completed due to a conflict with the current state of the
    ///     resource.
    /// </summary>
    Conflict,

    /// <summary>
    ///     Represents a failure error, indicating that an operation has
    ///     failed for some reason.
    /// </summary>
    Failure,

    /// <summary>
    ///     Represents a forbidden error, indicating that the request is
    ///     understood but refused by the server.
    /// </summary>
    Forbidden,

    /// <summary>
    ///     Represents an unauthorized error, indicating that the request
    ///     requires user authentication or the provided credentials are
    ///     invalid.
    /// </summary>
    Unauthorized
}