﻿using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;
using Nameless.Microservice.Web.Services;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class Post : IEndpoint {
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

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapPost($"{Internals.Endpoints.BaseApiPath}/todo", Handle)

                .Produces<TodoOutput>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError)

                .WithOpenApi()

                .WithName(nameof(Post))
                .WithDescription("Create a new To-Do")
                .WithSummary("Create a new To-Do")

                .WithApiVersionSet(builder.NewApiVersionSet("To-Do").Build())
                .HasApiVersion(1);

        #endregion
    }
}
