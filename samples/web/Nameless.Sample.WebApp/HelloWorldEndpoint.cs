using Nameless.Web.Http.Endpoints;

namespace Nameless.Sample.WebApp;

[Endpoint(route: "/hello-world")]
public partial class HelloWorldEndpoint {
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok(new { Message = "It works!" }));
    }
}
