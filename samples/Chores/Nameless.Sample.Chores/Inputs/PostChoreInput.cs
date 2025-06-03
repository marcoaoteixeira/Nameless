using Nameless.Validation;

namespace Nameless.Sample.Chores.Inputs;

[Validate]
public record PostChoreInput {
    public required string Description { get; init; }

    public bool Done { get; init; }

    public required DateOnly Date { get; init; }
}
