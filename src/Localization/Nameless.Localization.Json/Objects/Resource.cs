using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Json.Objects;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed record Resource {
    public static Resource Empty => new(string.Empty, string.Empty, [], isAvailable: false);

    public string Path { get; }

    public string Culture { get; }

    public Message[] Messages { get; }

    public bool IsAvailable { get; }

    private string DebuggerDisplayValue
        => $"{nameof(Path)}: {Path} | {nameof(Culture)}: {Culture} | {nameof(IsAvailable)}: {IsAvailable}";

    public Resource(string path, string culture, Message[] messages, bool isAvailable) {
        Path = Prevent.Argument.Null(path);
        Culture = Prevent.Argument.Null(culture);
        Messages = Prevent.Argument.Null(messages);
        IsAvailable = isAvailable;
    }

    public bool TryGetMessage(string id, [NotNullWhen(true)] out Message? output) {
        var current = Messages.SingleOrDefault(item => string.Equals(id, item.Id, StringComparison.Ordinal));

        output = current;

        return current is not null;
    }
}