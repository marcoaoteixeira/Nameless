using System.Diagnostics;

namespace Nameless.Infrastructure;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public sealed record Arg {
    private string DebuggerDisplayValue
        => $"[{Name}] {Value} ({Value?.GetType().Name})";

    public string Name { get; }

    public object? Value { get; }

    public Arg(string name, object? value) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);
        Value = value;
    }
}