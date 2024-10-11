using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web;
using Nameless.Web.Endpoints;
using HttpMethods = Nameless.Web.HttpMethods;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Delete : MinimalEndpointBase {
    public override string HttpMethod => HttpMethods.DELETE;
    
    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";

    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithOpenApi()
                  .WithName("Delete")
                  .WithDescription("Delete a checklist item")
                  .WithSummary("Delete a checklist item")
                  .WithGroupName("Checklist v1")
                  .Produces(StatusCodes.Status204NoContent)
                  .Produces(StatusCodes.Status404NotFound)
                  .WithApiVersionSet()
                  .HasApiVersion(1);

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