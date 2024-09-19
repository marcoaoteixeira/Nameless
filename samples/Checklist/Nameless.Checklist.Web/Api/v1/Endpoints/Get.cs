using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Api;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Get : IEndpoint {
    #region Public Static Methods

    public static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        IMediator mediator,
        IMapper mapper,
        CancellationToken cancellationToken
    ) {
        var request = new GetChecklistItemRequest {
            Id = id
        };
        var dto = await mediator.Send(request, cancellationToken);
        var output = mapper.Map<ChecklistItemOutput?>(dto);

        return output is not null
            ? Results.Ok(output)
            : Results.NotFound();
    }

    #endregion

    #region IMinimalEndpoint Members

    public string Name => nameof(Get);

    public string Summary => "Get a checklist item";

    public string Description => "Get a checklist item";

    public string Group => "Checklist";

    public int Version => 1;

    IEndpointConventionBuilder IEndpoint.Map(IEndpointRouteBuilder builder)
        => builder
           .MapGet($"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}", HandleAsync)

           .Produces(StatusCodes.Status200OK, typeof(ChecklistItemOutput))
           .Produces(StatusCodes.Status404NotFound)

           .ProducesProblem(StatusCodes.Status500InternalServerError);

    #endregion
}