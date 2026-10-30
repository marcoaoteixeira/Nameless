using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Registration;

namespace Nameless.Mediator;

/// <summary>
///     Registers the mediator, scanning the context assemblies.
/// </summary>
public class MediatorHostFeature : HostFeature<MediatorFeatureOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var configure = ConfigureFeature;
        var assemblies = context.Assemblies;

        builder.Services.RegisterMediator(
            AssemblyScanAwareHelper.MergeAssemblies(configure, assemblies)
        );
    }
}
