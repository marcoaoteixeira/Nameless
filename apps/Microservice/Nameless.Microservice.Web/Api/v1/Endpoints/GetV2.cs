using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class GetV2 : IEndpoint {
        #region Public Static Methods

        public static Task<IResult> HandleAsync([AsParameters] GetValues getValues) {
            var output = new GetOutput { Message = getValues.ToString() };
            var result = Results.Ok(output);

            return Task.FromResult(result);
        }

        #endregion

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet("api/v{version:apiVersion}/getwithobj", HandleAsync)

                .Produces<GetOutput>()

                .WithOpenApi()

                .WithName(nameof(GetV2))
                .WithDescription("Greetings API")
                .WithSummary("Greetings API")

                .WithApiVersionSet(builder.NewApiVersionSet("Greetings").Build())
                .HasApiVersion(2);

        #endregion
    }

    public record GetValues {
        public int Id { get; set; }
        public string? Message { get; set; }

        public override string ToString() => $"Id: {Id} / Message: {Message}";

        
    }
}
