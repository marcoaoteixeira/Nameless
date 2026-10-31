using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Windows.Dialogs.FileSystem;

/// <summary>
///     <see cref="IServiceCollection"/> extensions methods.
/// </summary>
public class FileSystemHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IFileSystemDialog, FileSystemDialog>();
    }
}
