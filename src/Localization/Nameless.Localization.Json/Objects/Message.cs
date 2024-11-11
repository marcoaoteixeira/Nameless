using System.Diagnostics;
using System.Text.Json.Serialization;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json.Objects;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
[JsonConverter(typeof(MessageJsonConverter))]
public sealed record Message {
    public static Message Empty => new(string.Empty, string.Empty);

    public string Id { get; }
    
    public string Text { get; }

    private string DebuggerDisplayValue
        => $"{Id} => {Text}";

    public Message(string id, string text) {
        Id = Prevent.Argument.Null(id);
        Text = Prevent.Argument.Null(text);
    }
}