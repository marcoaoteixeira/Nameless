using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Json.Objects;

[DebuggerDisplay("{DebuggerDisplayValue}")]
public sealed record Region {
    public static Region Empty => new(string.Empty, []);

    public string Name { get; }

    public Message[] Messages { get; }

    private string DebuggerDisplayValue
        => $"Region: {Name}";

    public Region(string name, Message[] messages) {
        Name = Prevent.Argument.Null(name);
        Messages = Prevent.Argument.Null(messages);
    }

    public bool TryGetMessage(string id, [NotNullWhen(true)] out Message? output) {
        var current = Messages.SingleOrDefault(item => id == item.ID);

        output = current;

        return current is not null;
    }
}