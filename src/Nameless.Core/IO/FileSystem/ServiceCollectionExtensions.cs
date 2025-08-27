using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.IO.FileSystem;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers the file system services.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configure">
    ///     The action used to configure the <see cref="FileSystemOptions"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="self"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection RegisterFileSystem(this IServiceCollection self, Action<FileSystemOptions>? configure = null) {
        Guard.Against.Null(self);

        self.Configure(configure ?? (_ => { }));
        self.TryAddSingleton<IFileSystem, FileSystemImpl>();

        return self;
    }
}
