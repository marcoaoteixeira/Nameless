using Nameless.Mediator.Requests;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

public sealed record CreateToDoItemRequest(string Summary, DateTime DueDate) : IRequest<CreateToDoItemResponse>;