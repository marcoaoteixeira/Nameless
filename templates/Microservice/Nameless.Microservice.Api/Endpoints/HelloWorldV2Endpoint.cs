using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Microservice.Api.Endpoints;

[Endpoint<Get>("/api/v{version:apiVersion}/hello-world", Tags = ["Greetings"])]
[Version("2")]
public class HelloWorldV2Endpoint {
    public Task<IResult> HandleAsync() {
        IResult result = TypedResults.Ok(new { Message = "Hello World V2!" });

        return Task.FromResult(result);
    }
}
