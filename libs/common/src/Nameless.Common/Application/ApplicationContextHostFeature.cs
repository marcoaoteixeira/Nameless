using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Application;

/// <summary>
///     Application Context Host Feature
/// </summary>
public class ApplicationContextHostFeature : HostFeature<ApplicationContextOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.ConfigureOptions<ApplicationContextOptions>(builder.Configuration)
               .TryAddSingleton<IApplicationContext, ApplicationContext>();
    }
}