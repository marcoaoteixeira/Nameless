using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.IO;
using Nameless.IO.Physical;

namespace Nameless.Hosting.Features.FileManager;

/// <summary>
///     Physical File Manager Host Feature
/// </summary>
public class PhysicalFileManagerHostFeature : HostFeature<PhysicalFileManagerFeatureOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        if (options.Roots.Count == 0) {
            builder.Services.TryAddSingleton<IFileManager>(
                new PhysicalFileManager(AppContext.BaseDirectory)
            );

            return;
        }

        foreach (var root in options.Roots) {
            builder.Services.TryAddKeyedSingleton<IFileManager>(
                serviceKey: root.Key,
                instance: new PhysicalFileManager(root.Value)
            );
        }
    }
}