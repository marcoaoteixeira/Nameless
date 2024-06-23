using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Validation.Abstractions;
using Nameless.Web.Infrastructure;

namespace Nameless.Checklist.Web.Api.v1.Endpoints {
    public sealed class Post : IMinimalEndpoint {
        #region Public Static Methods

        public static async Task<IResult> Handle(
            [FromBody] CreateChecklistItemInput input,
            IMediator mediator,
            IMapper mapper,
            CancellationToken cancellationToken
        ) {
            try {
                var request = mapper.Map<CreateChecklistItemRequest>(input);
                var dto = await mediator.Send(request, cancellationToken);
                var output = mapper.Map<ChecklistItemOutput>(dto);

                return Results.Ok(output);
            } catch (ValidationException ex) {
                return Results.ValidationProblem(ex.Result.ToDictionary(), statusCode: StatusCodes.Status400BadRequest);
            }
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(Post);

        public string Summary => "Create a new checklist item";

        public string Description => "Create a new checklist item";

        public string Group => "Checklist";

        public int Version => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapPost($"{Root.Endpoints.BASE_API_PATH}/checklist", Handle)

                .Produces(StatusCodes.Status200OK, typeof(ChecklistItemOutput))

                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
