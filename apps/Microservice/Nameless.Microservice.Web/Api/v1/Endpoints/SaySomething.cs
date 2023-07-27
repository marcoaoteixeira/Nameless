using Nameless.FluentValidation;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class SaySomething : IEndpoint {
        #region Public Static Methods

        public static Task<IResult> HandleAsync(SaySomethingInput input) {
            var result = Results.Ok(new SayHelloOutput { Message = $"You said {input.Message}" });

            return Task.FromResult(result);
        }

        #endregion

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapPost($"{Internals.Endpoints.BaseApiPath}/saysomething", HandleAsync)
                .Produces<SaySomethingOutput>()
                .AddEndpointFilter<ValidationEndpointFilter>()
                .WithApiVersionSet(builder.NewApiVersionSet("Greetings").Build())
                .HasApiVersion(1)
                .WithName(nameof(SaySomething))
                .WithDescription("Greetings API")
                .WithSummary("Greetings API")
                .WithOpenApi();

        #endregion
    }
}
