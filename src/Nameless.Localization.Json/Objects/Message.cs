using System.Diagnostics;
using System.Text.Json.Serialization;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json.Objects;

/// <summary>
/// Represents a localized message with an identifier and text.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
[JsonConverter(typeof(MessageJsonConverter))]
public sealed record Message {
    /// <summary>
    /// Gets an empty message with no identifier and text.
    /// </summary>
    public static Message Empty => new(string.Empty, string.Empty);

    /// <summary>
    /// Gets the identifier of the message.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the text of the message.
    /// </summary>
    public string Text { get; }

    private string DebuggerDisplayValue
        => $"{Id} => {Text}";

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/>.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <param name="text">The text.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="id"/> or
    ///     <paramref name="text"/> is <c>null</c>.
    /// </exception>
    public Message(string id, string text) {
        Id = Prevent.Argument.Null(id);
        Text = Prevent.Argument.Null(text);
    }

    /// <summary>
    /// Retrieves the message ID formatted with the provided arguments.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// The formatted message ID.
    /// </returns>
    public string GetId(object[] args) {
        return args.Length > 0 ? string.Format(Id, args) : Id;
    }

    /// <summary>
    /// Retrieves the message text formatted with the provided arguments.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// The formatted message text.
    /// </returns>
    public string GetText(object[] args) {
        return args.Length > 0 ? string.Format(Text, args) : Text;
    }
}