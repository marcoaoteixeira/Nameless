using Nameless.Microservices.Api.v1.Models;
using Nameless.Microservices.Services;
using Nameless.Web.Infrastructure;

namespace Nameless.Microservices.Api.v1.Endpoints {
    public sealed class List : IMinimalEndpoint {
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

        #region IMinimalEndpoint Members

        public string Name => nameof(List);

        public string Summary => "Get To-Do's";

        public string Description => "Get To-Do's";

        public string ApiSet => "To-Do";

        public int ApiVersion => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Root.Endpoints.BASE_API_PATH}/todo", Handle)

                .Produces<TodoOutput[]>();

        #endregion
    }
}
