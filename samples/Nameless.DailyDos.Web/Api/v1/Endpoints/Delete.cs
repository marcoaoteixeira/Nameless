using Microsoft.AspNetCore.Mvc;
using Nameless.Web.Infrastructure;

namespace Nameless.DailyDos.Web.Api.v1.Endpoints {
    public sealed class Delete : IMinimalEndpoint {
        #region Public Static Methods

        public static IResult Handle([FromRoute] Guid id) {
            try {
                

                return Results.NoContent();
            }
            catch (ArgumentException ex) {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception ex) {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(Delete);

        public string Summary => "Delete a To-Do";

        public string Description => "Delete a To-Do";

        public string Group => "To-Do";

        public int Version => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapDelete($"{Root.Endpoints.BASE_API_PATH}/todo/{{id}}", Handle)

                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
