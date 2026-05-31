using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;
using Nameless.Web.Http.Endpoints.Attributes.Cors;

namespace Nameless.Microservice.Api.Endpoints.Samples;

[Endpoint<Get>("/Cors", Name = "EndpointWithCors", Tags = ["Cors"])]
[UseCors("AllowEverything")]
public partial class EndpointWithCors {
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok(new { Message = "Cors Sample" }));
    }
}
