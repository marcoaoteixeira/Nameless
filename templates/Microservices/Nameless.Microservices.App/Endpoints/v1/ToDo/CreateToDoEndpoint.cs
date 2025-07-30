using Microsoft.AspNetCore.Mvc;
using Nameless.Mediator;
using Nameless.Microservices.App.Domains.UseCases;
using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Definitions;

using static Microsoft.AspNetCore.Http.TypedResults;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo;

public class CreateToDoEndpoint : IEndpoint {
    private readonly IMediator _mediator;

    public CreateToDoEndpoint(IMediator mediator) {
        _mediator = Prevent.Argument.Null(mediator);
    }

    public async Task<IResult> ExecuteAsync([FromBody] CreateToDoInput input, CancellationToken cancellationToken) {
        var request = new CreateToDoRequest {
            Summary = input.Summary ?? throw new ArgumentNullException(nameof(input)),
            DueDate = input.DueDate ?? throw new ArgumentNullException(nameof(input))
        };

        var todo = await _mediator.ExecuteAsync(request, cancellationToken);

        var output = new CreateToDoOutput {
            Id = todo.Id,
            Summary = todo.Summary,
            DueDate = todo.DueDate
        };

        return Ok(output);
    }

    public IEndpointDescriptor Describe() {
        return EndpointDescriptorBuilder.Create<CreateToDoEndpoint>()
                                        .Post("/", nameof(ExecuteAsync))
                                        .WithGroupName("todo")
                                        .AllowAnonymous()
                                        .WithDescription("Creates a new ToDo entry in the database.")
                                        .WithSummary("Create ToDo v1")
                                        .Produces<CreateToDoOutput>()
                                        .ProducesValidationProblem()
                                        .UseRateLimiting(Constants.RateLimitPolicies.SLIDING_WINDOW)
                                        .Build();
    }
}
