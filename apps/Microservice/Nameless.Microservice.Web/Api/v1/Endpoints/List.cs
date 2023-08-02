using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;
using Nameless.Microservice.Web.Services;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public sealed class List : IEndpoint {
        #region Public Static Methods

        public static IResult Handle([AsParameters] TodoQuery query, TodoService todoService) {
            var entities = todoService.List(query.DescriptionLike, query.FinishedBefore);
            var output = new List<TodoOutput>();

            foreach (var entity in entities) {
                output.Add(new() {
                    Id = entity.Id,
                    Description = entity.Description,
                    CreatedAt = entity.CreatedAt,
                    FinishedAt = entity.FinishedAt,
                });
            }

            return Results.Ok(output);
        }

        #endregion

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Internals.Endpoints.BaseApiPath}/todo", Handle)

                .Produces<TodoOutput[]>()

                .WithOpenApi()

                .WithName(nameof(List))
                .WithDescription("Get To-Do's")
                .WithSummary("Get To-Do's")

                .WithApiVersionSet(builder.NewApiVersionSet("To-Do").Build())
                .HasApiVersion(1);

        #endregion
    }
}
