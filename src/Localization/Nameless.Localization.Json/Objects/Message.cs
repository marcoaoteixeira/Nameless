using System.Diagnostics;

namespace Nameless.Localization.Json.Objects;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed record Message {
    public static Message Empty => new(string.Empty, string.Empty);

    public string ID { get; }
    
    public string Text { get; }

    private string DebuggerDisplayValue
        => $"{ID} => {Text}";

    public Message(string id, string text) {
        ID = Prevent.Argument.Null(id);
        Text = Prevent.Argument.Null(text);
    }
}