using Nameless.Validation;

namespace Nameless.Microservices.App.Endpoints.v1.HelloWorld;

[Validate]
public record HelloWorldInput {
    public string? Person { get; init; }
}
