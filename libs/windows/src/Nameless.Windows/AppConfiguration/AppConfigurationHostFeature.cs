using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Windows.AppConfiguration;

/// <summary>
///     App Configuration Host Feature
/// </summary>
public class AppConfigurationHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IAppConfigurationManager, AppConfigurationManager>();
    }
}