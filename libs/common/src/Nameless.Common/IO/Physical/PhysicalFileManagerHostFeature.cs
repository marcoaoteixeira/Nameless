using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.IO.Physical;

/// <summary>
///     Physical File Manager Host Feature
/// </summary>
public class PhysicalFileManagerHostFeature : HostFeature<PhysicalFileManagerOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);
        var root = string.IsNullOrWhiteSpace(options.Root)
            ? AppContext.BaseDirectory
            : options.Root;

        builder.Services.TryAddSingleton<PhysicalFileManagerFactory>();
        builder.Services.TryAddSingleton<IFileManager>(provider => provider.GetRequiredService<PhysicalFileManagerFactory>()
                                                                           .GetOrCreate(root));
    }
}