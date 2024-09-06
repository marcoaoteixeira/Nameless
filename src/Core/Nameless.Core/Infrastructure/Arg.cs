using System.Diagnostics;

namespace Nameless.Infrastructure;

[DebuggerDisplay("{DebuggerDisplay}")]
public sealed record Arg {
    private string DebuggerDisplay
        => $"[{Name}] {Value} ({Value.GetType().Name})";

    public string Name { get; }

    public object Value { get; }

    public Arg(string name, object value) {
        Name = Prevent.Argument.NullOrWhiteSpace(name, nameof(name));
        Value = Prevent.Argument.Null(value, nameof(value));
    }
}