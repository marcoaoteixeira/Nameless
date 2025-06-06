namespace Nameless.Sample.Chores.Dtos;

public record ChoreDto {
    public Guid ID { get; init; }

    public string Description { get; init; } = string.Empty;

    public bool Done { get; init; }

    public DateOnly Date { get; init; }
}