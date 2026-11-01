using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.Windows.Bootstrap;

/// <summary>
///     Bootstrapper Host Feature
/// </summary>
public class BootstrapperHostFeature : HostFeature<BootstrapperFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.TryAddTransient<IBootstrapper, Bootstrapper>();
        builder.Services.TryAddEnumerable(options.Steps.Select(
            implementation => ServiceDescriptor.Transient(typeof(IStep), implementation)
        ));
    }
}
