using Nameless.Mediator.Requests;
using Nameless.Microservices.App.Domains.Entities;

namespace Nameless.Microservices.App.Domains.UseCases;

public record CreateToDoRequest : IRequest<ToDo> {
    public required string Summary { get; init; }
    public required DateTime DueDate { get; init; }
}