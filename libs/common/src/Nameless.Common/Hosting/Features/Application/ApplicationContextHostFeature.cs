using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Application;

namespace Nameless.Hosting.Features.Application;

/// <summary>
///     Application Context Host Feature
/// </summary>
public class ApplicationContextHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services
               .ConfigureOptions<ApplicationContextOptions>(builder.Configuration)
               .TryAddSingleton<IApplicationContext, ApplicationContext>();
    }
}