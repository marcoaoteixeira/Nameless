using Nameless.Validation.Internals;

namespace Nameless.Validation;

/// <summary>
/// Represents a validation error entry.
/// </summary>
public sealed record ValidationEntry {
    /// <summary>
    /// Gets the entry code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the entry message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationEntry"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentException">
    /// if <paramref name="code"/> or
    /// <paramref name="message"/> is <c>null</c>, empty or white spaces.
    /// </exception>
    public ValidationEntry(string code, string message) {
        if (string.IsNullOrWhiteSpace(code)) {
            throw new ArgumentException(Constants.Messages.ParameterCannotBeStringNullOrWhiteSpace, nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message)) {
            throw new ArgumentException(Constants.Messages.ParameterCannotBeStringNullOrWhiteSpace, nameof(message));
        }

        Code = code;
        Message = message;
    }
}