using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nameless.DailyDos.Web.Api.v1.Models.Input;
using Nameless.DailyDos.Web.Api.v1.Models.Output;
using Nameless.DailyDos.Web.Domain.Requests;
using Nameless.Web.Infrastructure;

namespace Nameless.DailyDos.Web.Api.v1.Endpoints {
    public sealed class Post : IMinimalEndpoint {
        #region Public Static Methods

        public static async Task<IResult> Handle([FromBody] CreateTodoItemInput input, IMediator mediator, IMapper mapper, CancellationToken cancellationToken) {
            try {
                var request = mapper.Map<CreateTodoItemRequest>(input);
                var dto = await mediator.Send(request, cancellationToken);
                var output = mapper.Map<CreateTodoItemOutput>(dto);

                return Results.Ok(output);
            } catch (ValidationException ex) {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            } catch (Exception ex) {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(Post);

        public string Summary => "Create a new To-Do";

        public string Description => "Create a new To-Do";

        public string Group => "To-Do";

        public int Version => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapPost($"{Root.Endpoints.BASE_API_PATH}/todo", Handle)

                .Produces<CreateTodoItemOutput>()

                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
