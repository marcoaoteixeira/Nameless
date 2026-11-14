using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Windows.Dialogs.FileSystem;

namespace Nameless.Windows.Hosting.Features.Dialogs.FileSystem;

/// <summary>
///     <see cref="IServiceCollection"/> extensions methods.
/// </summary>
public class FileSystemDialogHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IFileSystemDialog, FileSystemDialog>();
    }
}
