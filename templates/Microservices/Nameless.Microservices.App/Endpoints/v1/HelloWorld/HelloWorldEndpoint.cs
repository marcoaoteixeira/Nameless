using Nameless.Web.Endpoints;
using Nameless.Web.Filters;

namespace Nameless.Microservices.App.Endpoints.v1.HelloWorld;

public class HelloWorldEndpoint : IEndpoint {
    public void Configure(IEndpointDescriptor descriptor) {
        descriptor
            .Get("/", HandleAsync)
            .AllowAnonymous()
            .WithFilter<ValidateRequestEndpointFilter>()
            .WithDescription("Greetings endpoint for the application.")
            .WithSummary("Hello World v1")
            .Produces<HelloWorldOutput>()
            .ProducesValidationProblem()
            .WithRateLimiting(Constants.RateLimitPolicies.SLIDING_WINDOW);
    }

    public Task<IResult> HandleAsync([AsParameters] HelloWorldInput input, CancellationToken cancellationToken) {
        var output = new HelloWorldOutput { Message = $"Hi {input.Person}!" };

        IResult result = TypedResults.Ok(output);

        return Task.FromResult(result);
    }
}
