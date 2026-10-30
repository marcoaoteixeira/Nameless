using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Resilience;

/// <summary>
///     Resilience Host Feature 
/// </summary>
public class ResilienceHostFeature : HostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddTransient<IRetryPipelineFactory, RetryPipelineFactory>();
    }
}
