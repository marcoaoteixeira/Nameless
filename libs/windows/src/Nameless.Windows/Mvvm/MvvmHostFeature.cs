using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.Windows.Mvvm;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public class MvvmHostFeature : HostFeature<MvvmFeatureOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature).WithAssemblies(context.Assemblies);

        builder.Services.TryAdd(options.ViewModels.Select(
            implementation => ServiceDescriptor.Transient(
                implementation,
                implementation
            )
        ));
    }
}