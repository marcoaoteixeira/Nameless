using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class GetV1 : IEndpoint {
        #region Public Static Methods

        public static Task<IResult> HandleAsync([FromQuery] string value) {
            var output = new GetOutput { Message = value };
            var result = Results.Ok(output);

            return Task.FromResult(result);
        }

        #endregion

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Internals.Endpoints.BaseApiPath}/get", HandleAsync)

                .Produces<GetOutput>()

                .WithOpenApi()

                .WithName(nameof(GetV1))
                .WithDescription("Greetings API")
                .WithSummary("Greetings API")
                
                .WithApiVersionSet(builder.NewApiVersionSet("Greetings").Build())
                .HasApiVersion(1);

        #endregion
    }
}
