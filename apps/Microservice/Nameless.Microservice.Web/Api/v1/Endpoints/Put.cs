using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;
using Nameless.Microservice.Web.Services;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class Put : IEndpoint {
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

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapPut($"{Internals.Endpoints.BaseApiPath}/todo", Handle)

                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError)

                .WithOpenApi()

                .WithName(nameof(Put))
                .WithDescription("Create a new To-Do")
                .WithSummary("Create a new To-Do")

                .WithApiVersionSet(builder.NewApiVersionSet("To-Do").Build())
                .HasApiVersion(1);

        #endregion
    }
}
