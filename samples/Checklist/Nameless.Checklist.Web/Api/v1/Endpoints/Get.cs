using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Get : MinimalEndpointBase {
    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";

    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithOpenApi()
                  .WithName("Get")
                  .WithDescription("Get a checklist item")
                  .WithSummary("Get a checklist item")
                  .WithGroupName("Checklist v1")
                  .Produces<ChecklistItemOutput>()
                  .Produces(StatusCodes.Status404NotFound)
                  .WithApiVersionSet()
                  .HasApiVersion(1);

    public override Delegate CreateDelegate() => HandleAsync;

    private static async Task<IResult> HandleAsync([FromRoute] Guid id,
                                                   [FromServices] IMediator mediator,
                                                   [FromServices] IMapper mapper,
                                                   CancellationToken cancellationToken) {
        var request = new GetChecklistItemRequest { Id = id };
        var dto = await mediator.Send(request, cancellationToken);
        var output = mapper.Map<ChecklistItemOutput?>(dto);

        return output is not null
            ? Results.Ok(output)
            : Results.NotFound();
    }
}