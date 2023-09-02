using AutoMapper;
using MediatR;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Requests;
using Nameless.Web.Infrastructure;

namespace Nameless.Checklist.Web.Api.v1.Endpoints {
    public sealed class List : IMinimalEndpoint {
        #region Public Static Methods

        public static async Task<IResult> HandleAsync(
            [AsParameters] ListChecklistItemsInput input,
            IMediator mediator,
            IMapper mapper,
            CancellationToken cancellationToken
        ) {
            var request = mapper.Map<ListChecklistItemsRequest>(input);
            var dtos = await mediator.Send(request, cancellationToken);
            var output = mapper.Map<ChecklistItemOutput[]>(dtos);

            return Results.Ok(output);
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(List);

        public string Summary => "Get checklist items";

        public string Description => "Get checklist items";

        public string Group => "Checklist";

        public int Version => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Root.Endpoints.BASE_API_PATH}/checklist", HandleAsync)

                .Produces(StatusCodes.Status200OK, typeof(ChecklistItemOutput[]))

                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
