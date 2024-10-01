﻿using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Validation.Abstractions;
using Nameless.Web.Endpoints;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Post : EndpointBase {
    public override string HttpMethod => Nameless.Web.Root.HttpMethods.POST;

    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    public override bool UseValidationFilter => false;

    public override OpenApiMetadata GetOpenApiMetadata()
        => new() {
            Name = "Post",
            Description = "Create a new checklist item",
            Summary = "Create a new checklist item",
            GroupName = "Checklist",
            Produces = [
                new Produces { StatusCode = HttpStatusCode.OK, ResponseType = typeof(ChecklistItemOutput) },
                new Produces { Type = ProducesType.ValidationProblems }
            ]
        };

    public override Delegate CreateDelegate() => HandleAsync;

    private static async Task<IResult> HandleAsync([FromBody] CreateChecklistItemInput input,
                                                   IMediator mediator,
                                                   IMapper mapper,
                                                   CancellationToken cancellationToken) {
        try {
            var request = mapper.Map<CreateChecklistItemRequest>(input);
            var dto = await mediator.Send(request, cancellationToken);
            var output = mapper.Map<ChecklistItemOutput>(dto);

            return Results.Ok(output);
        } catch (ValidationException ex) {
            return Results.ValidationProblem(ex.Result.ToDictionary());
        }
    }
}