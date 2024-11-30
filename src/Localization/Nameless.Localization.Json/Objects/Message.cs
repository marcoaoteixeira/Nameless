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

    public string GetId(object[] args)
        => args.Length > 0 ? string.Format(Id, args) : Id;

    public string GetText(object[] args)
        => args.Length > 0 ? string.Format(Text, args) : Text;
}