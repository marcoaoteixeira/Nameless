﻿namespace Nameless.Validation;

/// <summary>
/// Represents a validation error entry.
/// </summary>
public sealed record ValidationError {
    /// <summary>
    /// Gets the entry code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the entry message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationError"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentException">
    /// if <paramref name="code"/> or
    /// <paramref name="message"/> is <c>null</c>, empty or white spaces.
    /// </exception>
    public ValidationError(string code, string message) {
        Code = Prevent.Argument.NullOrWhiteSpace(code);
        Message = Prevent.Argument.NullOrWhiteSpace(message);
    }
}