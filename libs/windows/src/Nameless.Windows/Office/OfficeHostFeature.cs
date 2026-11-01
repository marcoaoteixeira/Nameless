using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Windows.Office;

/// <summary>
///     Microsoft Office Host Feature
/// </summary>
public class OfficeHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IWordApplication, WordApplication>();
    }
}
