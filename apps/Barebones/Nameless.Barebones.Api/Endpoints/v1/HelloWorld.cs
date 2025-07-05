using Nameless.Barebones.Application;
using Nameless.Web.Correlation;
using Nameless.Web.Endpoints;

namespace Nameless.Barebones.Api.Endpoints.v1;

public class HelloWorld : EndpointBase {
    private readonly ICorrelationAccessor _correlationAccessor;
    private readonly ILogger<HelloWorld> _logger;

    public HelloWorld(ICorrelationAccessor correlationAccessor, ILogger<HelloWorld> logger) {
        _correlationAccessor = correlationAccessor;
        _logger = logger;
    }

    public override void Configure(IEndpointDescriptor descriptor) {
        descriptor.Get("/", HandleAsync)
                  .AllowAnonymous()
                  .WithOutputCachePolicy(OutputCachePolicy.OneMinute.PolicyName);
    }

    public Task<IResult> HandleAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Greetings...");

        return OkAsync(new { Message = $"Hi! {_correlationAccessor.GetID()}" });
    }
}
