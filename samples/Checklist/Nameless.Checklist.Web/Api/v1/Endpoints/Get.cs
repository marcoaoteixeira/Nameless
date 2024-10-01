using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Get : EndpointBase {
    public override string HttpMethod => Nameless.Web.Root.HttpMethods.GET;

    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";

    public override bool UseValidationFilter => false;

    public override OpenApiMetadata GetOpenApiMetadata()
        => new() {
            Name = "Get",
            Description = "Get a checklist item",
            Summary = "Get a checklist item",
            GroupName = "Checklist",
            Produces = [
                new Produces { StatusCode = HttpStatusCode.OK, ResponseType = typeof(ChecklistItemOutput) },
                new Produces { StatusCode = HttpStatusCode.NotFound }
            ]
        };

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