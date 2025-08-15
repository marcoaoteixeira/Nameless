namespace Nameless.Microservices.App.Endpoints.v1.ToDo;

public record CreateToDoOutput {
    public required Guid Id { get; init; }
    public required string Summary { get; init; }
    public required DateTime DueDate { get; init; }
}