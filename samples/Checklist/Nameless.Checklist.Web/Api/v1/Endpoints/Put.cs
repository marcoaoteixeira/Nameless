using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Validation.Abstractions;
using Nameless.Web.Infrastructure;

namespace Nameless.Checklist.Web.Api.v1.Endpoints {
    public sealed class Put : IMinimalEndpoint {
        #region Public Static Methods

        public static async Task<IResult> HandleAsync(
            [FromBody] UpdateChecklistItemInput input,
            IMediator mediator,
            IMapper mapper,
            CancellationToken cancellationToken
        ) {
            try {
                var request = mapper.Map<UpdateChecklistItemRequest>(input);
                var dto = await mediator.Send(request, cancellationToken);
                var output = mapper.Map<ChecklistItemOutput>(dto);

                return Results.Ok(output);
            } catch (ValidationException ex) {
                return Results.ValidationProblem(ex.Result.ToDictionary(), statusCode: StatusCodes.Status400BadRequest);
            }
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(Put);

        public string Summary => "Update a checklist item";

        public string Description => "Update a checklist item";

        public string Group => "Checklist";

        public int Version => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapPut($"{Root.Endpoints.BASE_API_PATH}/checklist", HandleAsync)

                .Produces(StatusCodes.Status204NoContent)

                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
