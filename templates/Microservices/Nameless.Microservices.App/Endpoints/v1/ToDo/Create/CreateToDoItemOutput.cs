namespace Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

public sealed record CreateToDoItemOutput(Guid ID, string Summary, DateTime DueDate);