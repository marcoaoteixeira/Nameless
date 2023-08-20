using Microsoft.AspNetCore.Mvc;
using Nameless.Microservices.Api.v1.Models;
using Nameless.Microservices.Services;
using Nameless.Web.Infrastructure;

namespace Nameless.Microservices.Api.v1.Endpoints {
    public sealed class Get : IMinimalEndpoint {
        #region Public Static Methods

        public static IResult Handle([FromRoute] Guid id, TodoService todoService) {
            var entity = todoService.GetByID(id);

            if (entity is null) {
                return Results.NotFound();
            }

            var output = new TodoOutput {
                Id = entity.Id,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                FinishedAt = entity.FinishedAt,
            };

            return Results.Ok(output);
        }

        #endregion

        #region IMinimalEndpoint Members

        public string Name => nameof(Get);

        public string Summary => "Get a specific To-Do";

        public string Description => "Get a specific To-Do";

        public string ApiSet => "To-Do";

        public int ApiVersion => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Root.Endpoints.BASE_API_PATH}/todo/{{id}}", Handle)

                .Produces<TodoOutput>()
                .Produces(StatusCodes.Status404NotFound);

        #endregion
    }
}
