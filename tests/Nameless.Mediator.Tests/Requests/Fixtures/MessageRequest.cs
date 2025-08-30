namespace Nameless.Mediator.Requests.Fixtures;

public record MessageRequest(string Message) : IRequest<MessageResponse>;