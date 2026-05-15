using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Microservice.Api.Endpoints;

[Endpoint<Get>("/api/v{version:apiVersion}/hello-world", Tags = ["Greetings"])]
[Version("1", Deprecated = true)]
public partial class HelloWorldV1Endpoint {
    public Task<IResult> HandleAsync() {
        IResult result = TypedResults.Ok(new { Message = "Hello World!" });

        return Task.FromResult(result);
    }
}
