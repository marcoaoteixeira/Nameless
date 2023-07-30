using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Web.Api.v1.Models;

namespace Nameless.Microservice.Web.Api.v1.Endpoints {
    public class GetV1 : IEndpoint {
        #region Public Static Methods

        public static Task<IResult> HandleAsync([FromQuery] string value) {
            var output = new GetOutput { Message = value.ToString() };
            var result = Results.Ok(output);

            return Task.FromResult(result);
        }

        #endregion

        #region IEndpoint Members

        public void Map(IEndpointRouteBuilder builder)
            => builder
                .MapGet($"{Internals.Endpoints.BaseApiPath}/get", HandleAsync)
                .Produces<GetOutput>()
                .WithApiVersionSet(builder.NewApiVersionSet("Greetings").Build())
                .HasApiVersion(1)
                .WithName(nameof(GetV1))
                .WithDescription("Greetings API")
                .WithSummary("Greetings API")
                .WithOpenApi();

        #endregion
    }

    public record ValueFromQuery {
        public string? Value { get; set; }

        public override string ToString() => (Value ?? string.Empty).ToString();

        public static bool TryParse(string value, out ValueFromQuery output) {
            output = new() {  Value = value };
            return true;
        }

        public static ValueTask<ValueFromQuery?> BindAsync(HttpContext context, ParameterInfo parameter) {
            var value = context.Request.Query["value"];

            var result = new ValueFromQuery {
                Value = value,
            };

            return ValueTask.FromResult<ValueFromQuery?>(result);
        }
    }
}
