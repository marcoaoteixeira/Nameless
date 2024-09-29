using System.Net;
using Asp.Versioning.Builder;
using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Validation.Abstractions;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Post : IEndpoint {
    public string HttpMethod => System.Net.Http.HttpMethod.Post.Method;

    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    public string Name => "Post";

    public string Description => "Create a new checklist item";

    public string Summary => "Create a new checklist item";

    public string GroupName => "Checklist";

    public string[] Tags => [];

    public AcceptMetadata[] Accepts => [];

    public int Version => 1;

    public bool Deprecated => false;

    public int MapToVersion => 0;

    public ProducesMetadata[] Produces => [
        new() { StatusCode = HttpStatusCode.OK, ResponseType = typeof(ChecklistItemOutput) },
        new() { Type = ProducesResultType.ValidationProblems }
    ];

    public Delegate CreateDelegate() => async ([FromBody] CreateChecklistItemInput input,
                                               IMediator mediator,
                                               IMapper mapper,
                                               CancellationToken cancellationToken) => {
        try {
            var request = mapper.Map<CreateChecklistItemRequest>(input);
            var dto = await mediator.Send(request, cancellationToken);
            var output = mapper.Map<ChecklistItemOutput>(dto);

            return Results.Ok(output);
        } catch (ValidationException ex) {
            return Results.ValidationProblem(ex.Result.ToDictionary());
        }
    };
}