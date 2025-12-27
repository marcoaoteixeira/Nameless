using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Microservices.App.Endpoints.v1.HelloWorld;

public class HelloWorldEndpoint : IEndpoint {
    public Task<IResult> ExecuteAsync([AsParameters] HelloWorldInput input) {
        var output = new HelloWorldOutput { Message = $"Hi {input.Person}!" };

        IResult result = TypedResults.Ok(output);

        return Task.FromResult(result);
    }

    public IEndpointDescriptor Describe() {
        return EndpointDescriptorBuilder.Create<HelloWorldEndpoint>()
                                        .Get("/", nameof(ExecuteAsync))
                                        .AllowAnonymous()
                                        .WithDescription("Greetings endpoint for the application.")
                                        .WithSummary("Hello World v1")
                                        .UseRateLimiting(Constants.RateLimitPolicies.SLIDING_WINDOW)
                                        .Produces<HelloWorldOutput>()
                                        .Produces(statusCode: StatusCodes.Status429TooManyRequests)
                                        .ProducesValidationProblem()
                                        .Build();
    }
}