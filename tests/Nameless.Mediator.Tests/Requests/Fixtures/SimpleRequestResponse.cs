namespace Nameless.Mediator.Requests.Fixtures;

public class SimpleRequestResponse : IRequest<string> {
    public string Message { get; set; }
}