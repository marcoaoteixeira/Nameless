using Nameless.Web.Endpoints;

namespace Nameless.Identity.Web.Endpoints.v1;

public class HelloWorld : EndpointBase {
    private readonly ILogger<HelloWorld> _logger;

    public HelloWorld(ILogger<HelloWorld> logger) {
        _logger = logger;
    }

    public override void Configure(IEndpointDescriptor descriptor) {
        descriptor.Get("/", HandleAsync)
                  .AllowAnonymous()
                  .WithOutputCachePolicy(OutputCachePolicy.OneMinute.PolicyName);
    }

    public Task<IResult> HandleAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Greetings...");

        return OkAsync(new { Message = "Hi!" });
    }
}
