using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Infrastructure;

namespace Nameless.Checklist.Web.Api.v1.Endpoints;

public sealed class Delete : IMinimalEndpoint {
    #region Public Static Methods

    public static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        IMediator mediator,
        CancellationToken cancellationToken
    ) {
        var request = new DeleteChecklistItemRequest {
            Id = id
        };
        var result = await mediator.Send(request, cancellationToken);

        return result
            ? Results.NoContent()
            : Results.NotFound();
    }

    #endregion

    #region IMinimalEndpoint Members

    public string Name => nameof(Delete);

    public string Summary => "Delete a checklist item";

    public string Description => "Delete a checklist item";

    public string Group => "Checklist";

    public int Version => 1;

    IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
        => builder
           .MapDelete($"{Root.Endpoints.BASE_API_PATH}/checklist/{{id}}", HandleAsync)

           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound)

           .ProducesProblem(StatusCodes.Status500InternalServerError);

    #endregion
}