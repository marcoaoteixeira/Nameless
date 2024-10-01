using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v2.Models.Input;
using Nameless.Checklist.Web.Api.v2.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v2.Endpoints;

public sealed class List : EndpointBase {
    public override string HttpMethod => Nameless.Web.Root.HttpMethods.GET;

    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    public override bool UseValidationFilter => false;

    public override OpenApiMetadata GetOpenApiMetadata()
        => new() {
            Name = "List v2",
            Description = "Get checklist items",
            Summary = "Get checklist items",
            GroupName = "Checklist",
            Produces = [
                new Produces { StatusCode = HttpStatusCode.OK, ResponseType = typeof(ChecklistItemOutput[]) }
            ]
        };

    public override Versioning GetVersioningInfo()
        => new() { Version = 2 };

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