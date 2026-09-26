using System.Diagnostics;

namespace Nameless.ObjectModel;

/// <summary>
///     Represents an error.
/// </summary>
[DebuggerDisplay("{Flatten,nq}")]
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
    ///     Gets the error exception, if any.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    ///     Gets a readable version of the error.
    /// </summary>
    public string Flatten => string.IsNullOrWhiteSpace(Code)
        ? $"[{Type}] {Message}"
        : $"[{Type}] ({Code}) {Message}";

    /// <summary>
    ///     Do not use this type constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     if parameterless constructor is called.
    /// </exception>
    public Error() {
        throw new InvalidOperationException(message: "Do not use type constructor.");
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Error"/> class.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="code">
    ///     The code.
    /// </param>
    /// <param name="type">
    ///     The error type.
    /// </param>
    /// <param name="ex">
    ///     The exception.
    /// </param>
    internal Error(string message, string? code, ErrorType type, Exception? ex) {
        Message = Throws.When.NullOrWhiteSpace(message);
        Code = code;
        Type = type;
        Exception = ex;
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
    /// <param name="ex">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Validation(string message, string? code = null, Exception? ex = null) {
        return new Error(message, code, ErrorType.Validation, ex);
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
    /// <param name="ex">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Missing(string message, string? code = null, Exception? ex = null) {
        return new Error(message, code, ErrorType.Missing, ex);
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
    /// <param name="ex">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Conflict(string message, string? code = null, Exception? ex = null) {
        return new Error(message, code, ErrorType.Conflict, ex);
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
    /// <param name="ex">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Failure(string message, string? code = null, Exception? ex = null) {
        return new Error(message, code, ErrorType.Failure, ex);
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
    /// <param name="ex">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Forbidden(string message, string? code = null, Exception? ex = null) {
        return new Error(message, code, ErrorType.Forbidden, ex);
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
    /// <param name="ex">
    ///     The exception.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Error" /> class.
    /// </returns>
    public static Error Unauthorized(string message, string? code = null, Exception? ex = null) {
        return new Error(message, code, ErrorType.Unauthorized, ex);
    }
}