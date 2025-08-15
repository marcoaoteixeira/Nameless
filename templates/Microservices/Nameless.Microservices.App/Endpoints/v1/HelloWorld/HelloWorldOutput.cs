namespace Nameless.Microservices.App.Endpoints.v1.HelloWorld;

public record HelloWorldOutput {
    public required string Message { get; init; }
}