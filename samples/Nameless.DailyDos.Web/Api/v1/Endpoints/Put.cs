using Microsoft.AspNetCore.Mvc;
using Nameless.DailyDos.Web.Api.v1.Models.Input;
using Nameless.Web.Infrastructure;

namespace Nameless.DailyDos.Web.Api.v1.Endpoints {
    public sealed class Put : IMinimalEndpoint {
        #region Public Static Methods

        public static IResult Handle([FromBody] UpdateTodoItemInput input) {
            try {
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

        public string Group => "To-Do";

        public int Version => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapPut($"{Root.Endpoints.BASE_API_PATH}/todo", Handle)

                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
