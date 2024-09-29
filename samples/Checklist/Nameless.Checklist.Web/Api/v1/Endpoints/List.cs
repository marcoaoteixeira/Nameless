using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Api;
using HttpMethod = Nameless.Web.Api.HttpMethod;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class List : IEndpoint {
    public HttpMethod Method => HttpMethod.Get;

    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    [EndpointName(nameof(List))]
    [EndpointSummary("Get checklist items")]
    [EndpointDescription("Get checklist items")]
    [EndpointGroupName("Checklist")]
    [ApiVersion(1)]
    public Delegate GetHandler() => Handle;

    public async Task<IResult> Handle([AsParameters] ListChecklistItemsInput input,
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