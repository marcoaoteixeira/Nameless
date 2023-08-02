using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;
using Nameless.Microservice.Web.Services;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public sealed class Get : IEndpoint {
        #region Public Static Methods

        public static IResult Handle([FromRoute] Guid id, TodoService todoService) {
            var entity = todoService.GetByID(id);

            if (entity == null) {
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

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Internals.Endpoints.BaseApiPath}/todo/{{id}}", Handle)

                .Produces<TodoOutput>()
                .Produces(StatusCodes.Status404NotFound)

                .WithOpenApi()

                .WithName(nameof(Get))
                .WithDescription("Get a specific To-Do")
                .WithSummary("Get a specific To-Do")
                
                .WithApiVersionSet(builder.NewApiVersionSet("To-Do").Build())
                .HasApiVersion(1);

        #endregion
    }
}
