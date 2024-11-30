using Nameless.Web.Endpoints;

namespace Nameless.ApiVersioning.Sample.Web.Endpoints.v0;

public sealed class HelloWorld : MinimalEndpointBase {
    public override string RoutePattern => "api/helloworld";
    
    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithName("HelloWorld_V0");

    public override Delegate CreateDelegate() => HandleAsync;

    private Task<IResult> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Results.Ok(new { message = "v0: Hello World!" }));
}
