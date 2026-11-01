using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.Workers;

/// <summary>
///     Periodic Worker App Host Feature.
/// </summary>
public sealed class PeriodicWorkerHostFeature : HostFeature<PeriodicWorkerFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.TryAddEnumerable(options.Workers.Select(
            implementation => ServiceDescriptor.Singleton(
                typeof(IHostedService),
                implementation
            )
        ));
    }
}