using System.Data;
using System.Diagnostics;

namespace Nameless.Data;

/// <summary>
///     Represents a parameter for database commands, encapsulating its name, value, and type.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public readonly record struct Parameter {
    private string DebuggerDisplayValue
        => $"[{Type}] {Name} => {Value ?? "NULL"}";

    /// <summary>
    ///     Gets the name of the parameter.
    /// </summary>
    public string Name { get; }
    /// <summary>
    ///     Gets the value of the parameter.
    /// </summary>
    public object? Value { get; }
    /// <summary>
    ///     Gets the type of the parameter.
    /// </summary>
    public DbType Type { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Parameter"/>.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="type">The type of the parameter.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public Parameter(string name, object? value, DbType type = DbType.String) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);
        Value = value;
        Type = type;
    }
}