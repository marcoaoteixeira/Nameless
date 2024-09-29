using System.Net;
using Asp.Versioning.Builder;
using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Get : IEndpoint {
    public string HttpMethod => System.Net.Http.HttpMethod.Get.Method;

    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}";

    public string Name => "Get";

    public string Description => "Get a checklist item";

    public string Summary => "Get a checklist item";

    public string GroupName => "Checklist";

    public string[] Tags => [];

    public AcceptMetadata[] Accepts => [];

    public int Version => 1;

    public bool Deprecated => false;

    public int MapToVersion => 0;

    public ProducesMetadata[] Produces => [
        new() { StatusCode = HttpStatusCode.OK, ResponseType = typeof(ChecklistItemOutput) },
        new() { StatusCode = HttpStatusCode.NotFound }
    ];

    public Delegate CreateDelegate() => async ([FromRoute] Guid id,
                                               [FromServices] IMediator mediator,
                                               [FromServices] IMapper mapper,
                                               CancellationToken cancellationToken) => {
        var request = new GetChecklistItemRequest { Id = id };
        var dto = await mediator.Send(request, cancellationToken);
        var output = mapper.Map<ChecklistItemOutput?>(dto);

        return output is not null
            ? Results.Ok(output)
            : Results.NotFound();
    };
}