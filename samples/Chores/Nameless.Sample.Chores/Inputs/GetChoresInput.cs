namespace Nameless.Sample.Chores.Inputs;

public record GetChoresInput {
    public string? Description { get; init; }

    public bool? Done { get; init; }

    public DateOnly? Date { get; init; }
}
