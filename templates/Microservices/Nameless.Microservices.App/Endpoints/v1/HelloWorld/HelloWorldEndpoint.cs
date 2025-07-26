using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Filters;

namespace Nameless.Microservices.App.Endpoints.v1.HelloWorld;

public class HelloWorldEndpoint : IEndpoint<HelloWorldInput> {
    public Task<IResult> ExecuteAsync([AsParameters] HelloWorldInput input, CancellationToken cancellationToken) {
        var output = new HelloWorldOutput { Message = $"Hi {input.Person}!" };

        IResult result = TypedResults.Ok(output);

        return Task.FromResult(result);
    }

    public IEndpointDescriptor Describe() {
        return EndpointDescriptorBuilder.Create()
                                        .Get("/")
                                        .AllowAnonymous()
                                        .WithFilter<ValidateRequestEndpointFilter>()
                                        .WithDescription("Greetings endpoint for the application.")
                                        .WithSummary("Hello World v1")
                                        .Produces<HelloWorldOutput>()
                                        .Produces(statusCode: StatusCodes.Status429TooManyRequests)
                                        .ProducesValidationProblem()
                                        .WithRateLimiting(Constants.RateLimitPolicies.SLIDING_WINDOW)
                                        .Build();
    }
}
