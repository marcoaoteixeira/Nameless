using Nameless.Web.Http.Endpoints;

namespace Nameless.Microservices.Api;

[Endpoint<Get>(route: "/hello-world")]
public partial class HelloWorldEndpoint {
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok("Hello World!"));
    }
}
