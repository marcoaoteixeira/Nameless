using Microsoft.AspNetCore.Mvc;
using Nameless.Microservices.Api.v1.Models;
using Nameless.Microservices.Services;
using Nameless.Web.Infrastructure;

namespace Nameless.Microservices.Api.v1.Endpoints {
    public sealed class Put : IMinimalEndpoint {
        #region Public Static Methods

        public static IResult Handle([FromBody] UpdateTodoInput input, TodoService todoService) {
            try {
                todoService.SetDescription(input.Id, input.Description);

                if (input.FinishedAt.GetValueOrDefault() != DateTime.MinValue) {
                    todoService.SetFinished(input.Id);
                }

                return Results.NoContent();
            } catch (ArgumentException ex) {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            } catch (Exception ex) {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(Put);

        public string Summary => "Update a To-Do";

        public string Description => "Update a To-Do";

        public string ApiSet => "To-Do";

        public int ApiVersion => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapPut($"{Root.Endpoints.BASE_API_PATH}/todo", Handle)

                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
