namespace Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

public sealed record CreateToDoItemInput(string? Summary, DateTime DueDate);