namespace Nameless.ObjectModel;

/// <summary>
///     Represents an error.
/// </summary>
public readonly record struct Error {
    /// <summary>
    ///     Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the error code.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    ///     Gets the error type.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    ///     Do not use this type constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     if parameterless constructor is called.
    /// </exception>
    public Error() {
        throw new InvalidOperationException(message: "Do not use type constructor.");
    }

    private Error(string message, string? code, ErrorType type) {
        Message = message;
        Code = code;
        Type = type;
    }

    /// <summary>
    ///     Creates a new validation <see cref="Error" /> type with the
    ///     specified message and type.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="code">
    ///     The error code.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Validation(string message, string? code = null) {
        return new Error(message, code, ErrorType.Validation);
    }

    /// <summary>
    ///     Creates a new missing <see cref="Error" /> type with the
    ///     specified message and type.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="code">
    ///     The error code.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Missing(string message, string? code = null) {
        return new Error(message, code, ErrorType.Missing);
    }

    /// <summary>
    ///     Creates a new conflict <see cref="Error" /> type with the
    ///     specified message and type.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="code">
    ///     The error code.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Conflict(string message, string? code = null) {
        return new Error(message, code, ErrorType.Conflict);
    }

    /// <summary>
    ///     Creates a new failure <see cref="Error" /> type with the
    ///     specified message and type.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="code">
    ///     The error code.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Failure(string message, string? code = null) {
        return new Error(message, code, ErrorType.Failure);
    }

    /// <summary>
    ///     Creates a new forbidden <see cref="Error" /> type with the
    ///     specified message and type.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="code">
    ///     The error code.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Forbidden(string message, string? code = null) {
        return new Error(message, code, ErrorType.Forbidden);
    }

    /// <summary>
    ///     Creates a new unauthorized <see cref="Error" /> type with the
    ///     specified message and type.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="code">
    ///     The error code.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Unauthorized(string message, string? code = null) {
        return new Error(message, code, ErrorType.Unauthorized);
    }
}