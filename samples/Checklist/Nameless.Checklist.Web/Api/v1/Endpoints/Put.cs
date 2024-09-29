using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Validation.Abstractions;
using Nameless.Web.Api;
using HttpMethod = Nameless.Web.Api.HttpMethod;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Put : IEndpoint {
    public HttpMethod Method => HttpMethod.Put;
    
    public string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    [EndpointName(nameof(Put))]
    [EndpointSummary("Update a checklist item")]
    [EndpointDescription("Update a checklist item")]
    [EndpointGroupName("Checklist")]
    [ApiVersion(1)]
    public Delegate GetHandler() => async (
        [FromBody] UpdateChecklistItemInput input,
        IMediator mediator,
        IMapper mapper,
        CancellationToken cancellationToken
    ) => {
        try {
            var request = mapper.Map<UpdateChecklistItemRequest>(input);
            var dto = await mediator.Send(request, cancellationToken);
            var output = mapper.Map<ChecklistItemOutput>(dto);

            return Results.Ok(output);
        } catch (ValidationException ex) {
            return Results.ValidationProblem(ex.Result.ToDictionary(), statusCode: StatusCodes.Status400BadRequest);
        }
    };
}