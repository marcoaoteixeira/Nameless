using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Delete : EndpointBase {
    public override string HttpMethod => Nameless.Web.Root.HttpMethods.DELETE;
    
    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";

    public override bool UseValidationFilter => false;

    public override OpenApiMetadata GetOpenApiMetadata()
        => new() {
            Name = "Delete",
            Description = "Delete a checklist item",
            Summary = "Delete a checklist item",
            GroupName = "Checklist",
            Produces = [
                new Produces { StatusCode = HttpStatusCode.NoContent },
                new Produces { StatusCode = HttpStatusCode.NotFound }
            ]
        };

    public override Delegate CreateDelegate() => HandleAsync;

    private static async Task<IResult> HandleAsync([FromRoute] Guid id,
                                                   [FromServices] IMediator mediator,
                                                   CancellationToken cancellationToken) {
        var request = new DeleteChecklistItemRequest { Id = id };
        var result = await mediator.Send(request, cancellationToken);

        return result
            ? Results.NoContent()
            : Results.NotFound();
    }
}