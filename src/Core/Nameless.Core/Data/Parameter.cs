using System.Data;
using System.Diagnostics;

namespace Nameless.Data;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public readonly record struct Parameter {
    private string DebuggerDisplayValue
        => $"[{Type}] {Name} => {Value ?? "NULL"}";

    public string Name { get; }
    public object? Value { get; }
    public DbType Type { get; }

    public Parameter(string name, object? value, DbType type = DbType.String) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);
        Value = value;
        Type = type;
    }
}