using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Api;
using HttpMethod = Nameless.Web.Api.HttpMethod;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Delete : IEndpoint {
    public HttpMethod Method => HttpMethod.Delete;

    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";

    [EndpointName(nameof(Delete))]
    [EndpointSummary("Delete a checklist item")]
    [EndpointDescription("Delete a checklist item")]
    [EndpointGroupName("Checklist")]
    [ApiVersion(1)]
    public Delegate GetHandler() => async (
        [FromRoute] Guid id,
        IMediator mediator,
        CancellationToken cancellationToken) => {
        var request = new DeleteChecklistItemRequest { Id = id };
        var result = await mediator.Send(request, cancellationToken);
        
        return result
            ? Results.NoContent()
            : Results.NotFound();
    };
}