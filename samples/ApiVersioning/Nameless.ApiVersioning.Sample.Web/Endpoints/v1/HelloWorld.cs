using Nameless.Web.Endpoints;

namespace Nameless.ApiVersioning.Sample.Web.Endpoints.v1;

public sealed class HelloWorld : MinimalEndpointBase {
    public override string RoutePattern => $"{Constants.BaseUrl}/helloworld";
    
    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithName("HelloWorld_V1")
                  .WithApiVersionSet()
                  .HasApiVersion(1);

    public override Delegate CreateDelegate() => HandleAsync;

    private Task<IResult> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Results.Ok(new { message = "v1: Hello World!" }));
}
