using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class List : MinimalEndpointBase {
    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithOpenApi()
                  .WithName("List")
                  .WithDescription("List checklist items")
                  .WithSummary("List checklist items")
                  .WithGroupName("Checklist v1")
                  .Produces<ChecklistItemOutput[]>()
                  .WithApiVersionSet()
                  .HasApiVersion(1)
                  .HasDeprecatedApiVersion(1);

    public override Delegate CreateDelegate() => HandleAsync;

    private static async Task<IResult> HandleAsync([AsParameters] ListChecklistItemsInput input,
                                                   [FromServices] IMediator mediator,
                                                   [FromServices] IMapper mapper,
                                                   CancellationToken cancellationToken
    ) {
        var request = mapper.Map<ListChecklistItemsRequest>(input);
        var dtos = await mediator.Send(request, cancellationToken);
        var output = mapper.Map<ChecklistItemOutput[]>(dtos);

        return Results.Ok(output);
    }
}