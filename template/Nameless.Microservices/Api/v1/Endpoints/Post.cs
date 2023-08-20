using Microsoft.AspNetCore.Mvc;
using Nameless.Microservices.Api.v1.Models;
using Nameless.Microservices.Services;
using Nameless.Web.Infrastructure;

namespace Nameless.Microservices.Api.v1.Endpoints {
    public sealed class Post : IMinimalEndpoint {
        #region Public Static Methods

        public static IResult Handle([FromBody] CreateTodoInput input, TodoService todoService) {
            try {
                var entity = todoService.Create(input.Description);
                var output = new TodoOutput {
                    Id = entity.Id,
                    Description = entity.Description,
                    CreatedAt = entity.CreatedAt,
                    FinishedAt = entity.FinishedAt,
                };

                return Results.Ok(output);
            } catch (ArgumentException ex) {
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

        public string ApiSet => "To-Do";

        public int ApiVersion => 1;

        IEndpointConventionBuilder IMinimalEndpoint.Map(IEndpointRouteBuilder builder)
            => builder
                .MapPost($"{Root.Endpoints.BASE_API_PATH}/todo", Handle)

                .Produces<TodoOutput>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

        #endregion
    }
}
