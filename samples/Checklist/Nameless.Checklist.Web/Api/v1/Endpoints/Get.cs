using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Api;
using HttpMethod = Nameless.Web.Api.HttpMethod;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Get : IEndpoint {
    public HttpMethod Method => HttpMethod.Get;

    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";

    [EndpointName(nameof(Get))]
    [EndpointSummary("Get a checklist item")]
    [EndpointDescription("Get a checklist item")]
    [EndpointGroupName("Checklist")]
    [ApiVersion(1)]
    public Delegate GetHandler() => async (
        [FromRoute] Guid id,
        IMediator mediator,
        IMapper mapper,
        CancellationToken cancellationToken) => {
        var request = new GetChecklistItemRequest { Id = id };
        var dto = await mediator.Send(request, cancellationToken);
        var output = mapper.Map<ChecklistItemOutput?>(dto);

        return output is not null
            ? Results.Ok(output)
            : Results.NotFound();
    };
}