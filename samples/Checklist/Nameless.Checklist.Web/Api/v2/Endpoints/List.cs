using System.Net;
using Asp.Versioning.Builder;
using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v2.Models.Input;
using Nameless.Checklist.Web.Api.v2.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v2.Endpoints;

public sealed class List : IEndpoint {
    public string HttpMethod => System.Net.Http.HttpMethod.Get.Method;

    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    public string Name => "List v2";

    public string Description => "Get checklist items";

    public string Summary => "Get checklist items";

    public string GroupName => "Checklist";

    public string[] Tags => [];

    public AcceptMetadata[] Accepts => [];

    public int Version => 2;

    public bool Deprecated => false;

    public int MapToVersion => 0;

    public ProducesMetadata[] Produces => [
        new() { StatusCode = HttpStatusCode.OK, ResponseType = typeof(ChecklistItemOutput[]) }
    ];

    public Delegate CreateDelegate() => async([AsParameters] ListChecklistItemsInput input,
                                              [FromServices] IMediator mediator,
                                              [FromServices] IMapper mapper,
                                              CancellationToken cancellationToken) => {
        var request = mapper.Map<ListChecklistItemsRequest>(input);
        var dtos = await mediator.Send(request, cancellationToken);
        var output = mapper.Map<ChecklistItemOutput[]>(dtos);

        return Results.Ok(output);
    };
}