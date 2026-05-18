using Microsoft.AspNetCore.Cors;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Microservice.Api.Endpoints.Samples;

[Endpoint<Get>("/Cors", Name = "EndpointWithCors", Tags = ["Cors"])]
[EnableCors("AllowEverything")]
public partial class EndpointWithCors {
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok(new { Message = "Cors Sample" }));
    }
}
