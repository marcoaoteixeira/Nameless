using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Resilience;

namespace Nameless.Hosting.Features.Resilience;

/// <summary>
///     Resilience Host Feature 
/// </summary>
public class ResilienceHostFeature : HostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddTransient<IRetryPipelineFactory, RetryPipelineFactory>();
    }
}
