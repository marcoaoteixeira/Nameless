using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Nameless.Hosting.Features;

/// <summary>
///     Registers the services every web host needs: options, problem
///     details, the HTTP context accessor and <see cref="TimeProvider"/>.
/// </summary>
public class CommonHostFeature : HostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.AddOptions();

        // TryAdd so tests can replace it with a fake time provider.
        builder.Services.TryAddSingleton(TimeProvider.System);
    }
}
