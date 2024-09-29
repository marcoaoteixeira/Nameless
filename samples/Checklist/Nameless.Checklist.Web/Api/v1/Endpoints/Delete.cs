using System.Net;
using Asp.Versioning;
using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Delete : IEndpoint {
    public string HttpMethod => System.Net.Http.HttpMethod.Delete.Method;
    
    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";
    
    public string Name => "Delete";
    
    public string Description => "Delete a checklist item";
    
    public string Summary => "Delete a checklist item";
    
    public string GroupName => "Checklist";
    
    public string[] Tags => [];
    
    public AcceptMetadata[] Accepts => [];

    public int Version => 1;

    public bool Deprecated => false;

    public int MapToVersion => 0;

    public ProducesMetadata[] Produces => [
        new() { StatusCode = HttpStatusCode.NoContent },
        new() { StatusCode = HttpStatusCode.NotFound }
    ];

    public Delegate CreateDelegate() => async ([FromRoute] Guid id,
                                               [FromServices] IMediator mediator,
                                               CancellationToken cancellationToken) => {
        var request = new DeleteChecklistItemRequest { Id = id };
        var result = await mediator.Send(request, cancellationToken);

        return result
            ? Results.NoContent()
            : Results.NotFound();
    };
}