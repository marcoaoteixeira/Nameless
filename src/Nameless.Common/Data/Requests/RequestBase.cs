using System.Data;

namespace Nameless.Data.Requests;

/// <summary>
///     Represents a base request.
/// </summary>
public record RequestBase {
    /// <summary>
    ///     Gets the SQL command text.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    ///     Gets the SQL command type.
    /// </summary>
    public CommandType Type { get; init; }

    /// <summary>
    ///     Gets the SQL command parameters.
    /// </summary>
    public ParameterCollection Parameters { get; init; } = [];
}