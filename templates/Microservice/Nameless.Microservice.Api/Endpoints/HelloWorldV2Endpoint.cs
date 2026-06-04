using Microsoft.AspNetCore.Mvc;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Microservice.Api.Endpoints;

[Endpoint<Get>("/api/v{version:apiVersion}/hello-world/{name}", Tags = ["Greetings"], Version = "2")]
public partial class HelloWorldV2Endpoint {
    public Task<IResult> HandleAsync([FromRoute] string name) {
        IResult result = TypedResults.Ok(new { Message = $"Hello World {name}!" });

        return Task.FromResult(result);
    }
}
