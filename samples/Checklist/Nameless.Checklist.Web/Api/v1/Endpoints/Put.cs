using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Validation;
using Nameless.Web;
using Nameless.Web.Endpoints;
using HttpMethods = Nameless.Web.HttpMethods;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Put : MinimalEndpointBase {
    public override string HttpMethod => HttpMethods.PUT;

    public override string RoutePattern => $"{Root.Endpoints.BASE_API_PATH}/checklist";

    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithOpenApi()
                  .WithName("Put")
                  .WithDescription("Update a checklist item")
                  .WithSummary("Update a checklist item")
                  .WithGroupName("Checklist v1")
                  .Produces<ChecklistItemOutput>()
                  .ProducesValidationProblem()
                  .WithApiVersionSet()
                  .HasApiVersion(1);

    public override Delegate CreateDelegate() => HandleAsync;

    private static async Task<IResult> HandleAsync([FromBody] CreateChecklistItemInput input,
                                                   IMediator mediator,
                                                   IMapper mapper,
                                                   CancellationToken cancellationToken) {
        try {
            var request = mapper.Map<UpdateChecklistItemRequest>(input);
            var dto = await mediator.Send(request, cancellationToken);
            var output = mapper.Map<ChecklistItemOutput>(dto);

            return Results.Ok(output);
        } catch (ValidationException ex) {
            return Results.ValidationProblem(ex.Result.ToDictionary());
        }
    }
}