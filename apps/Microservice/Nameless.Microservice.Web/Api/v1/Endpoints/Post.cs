using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class Post : IEndpoint {
        #region Public Static Methods

        public static Task<IResult> HandleAsync([FromBody] DataInfo value) {
            var output = new PostOutput {
                Id = value.Id,
                Description = value.Description,
                Message = value.Message
            };
            var result = Results.Ok(output);

            return Task.FromResult(result);
        }

        #endregion

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapPost($"{Internals.Endpoints.BaseApiPath}/post", HandleAsync)
                .Produces<GetOutput>()
                .WithApiVersionSet(builder.NewApiVersionSet("Greetings").Build())
                .HasApiVersion(2)
                .WithName(nameof(Post))
                .WithDescription("Greetings API")
                .WithSummary("Greetings API")
                .WithOpenApi();

        #endregion
    }

    public record DataInfo {
        public string Id { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
