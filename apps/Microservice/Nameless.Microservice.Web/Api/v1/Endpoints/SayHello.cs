using Microsoft.AspNetCore.Authorization;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Services;
using Nameless.Microservice.Web.Api.v1.Models;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class SayHello : IEndpoint {
        #region Public Static Methods

        public static Task<IResult> HandleAsync(IJwtService jwtService) {
            var result = Results.Ok(new SayHelloOutput { Message = $"JWTService: {jwtService != null}" });

            return Task.FromResult(result);
        }

        #endregion

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Internals.Endpoints.BaseApiPath}/sayhello", HandleAsync)
                .Produces<SayHelloOutput>()
                .RequireAuthorization()
                .WithApiVersionSet(builder.NewApiVersionSet("Greetings").Build())
                .HasApiVersion(1)
                .WithName(nameof(SayHello))
                .WithDescription("Greetings API")
                .WithSummary("Greetings API")
                .WithOpenApi();

        #endregion
    }
}
