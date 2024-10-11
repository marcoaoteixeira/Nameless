using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v2.Models.Input;
using Nameless.Checklist.Web.Api.v2.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web;
using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Impl;
using HttpMethods = Nameless.Web.HttpMethods;

namespace Nameless.Checklist.Web.Api.v2.Endpoints;

public sealed class List : MinimalEndpointBase {
    public override string HttpMethod => HttpMethods.GET;

    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";
    
    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithOpenApi()
                  .WithName("List v2")
                  .WithDescription("Get checklist items")
                  .WithSummary("Get checklist items")
                  .WithGroupName("Checklist v2")
                  .Produces<ChecklistItemOutput>()
                  .WithApiVersionSet()
                  .HasApiVersion(2);

    public override Delegate CreateDelegate() => HandleAsync;

    private static async Task<IResult> HandleAsync([AsParameters] ListChecklistItemsInput input,
                                                   [FromServices] IMediator mediator,
                                                   [FromServices] IMapper mapper,
                                                   CancellationToken cancellationToken) {
        var request = mapper.Map<ListChecklistItemsRequest>(input);
        var dtos = await mediator.Send(request, cancellationToken);
        var output = mapper.Map<ChecklistItemOutput[]>(dtos);

        return Results.Ok(output);
    }
}