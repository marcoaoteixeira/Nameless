using System.Data;

namespace Nameless.Data.Requests;

public record RequestBase {
    public required string Text { get; init; }

    public CommandType Type { get; init; }

    public ParameterCollection Parameters { get; init; } = [];
}