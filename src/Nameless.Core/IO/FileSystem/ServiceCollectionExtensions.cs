using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Infrastructure;

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
    ///     if <paramref name="self"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection RegisterFileSystem(this IServiceCollection self, Action<FileSystemOptions>? configure = null) {
        Guard.Against.Null(self);

        self.Configure(configure ?? (_ => { }));
        self.TryAddSingleton<IFileSystem>(ResolveFileSystem);

        return self;
    }

    /// <summary>
    ///     Registers the file system services.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if
    ///         <paramref name="self"/> or
    ///         <paramref name="configuration"/>
    ///     is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection RegisterFileSystem(this IServiceCollection self, IConfiguration configuration) {
        Guard.Against.Null(self);
        Guard.Against.Null(configuration);

        var section = configuration.GetSection(nameof(FileSystemOptions));

        self.Configure<FileSystemOptions>(section);
        self.TryAddSingleton<IFileSystem>(ResolveFileSystem);

        return self;
    }

    private static FileSystemImpl ResolveFileSystem(IServiceProvider provider) {
        var options = provider.GetOptions<FileSystemOptions>();

        if (!string.IsNullOrWhiteSpace(options.Value.Root)) {
            return new FileSystemImpl(options);
        }

        var applicationContext = provider.GetService<IApplicationContext>();

        options.Value.Root = applicationContext?.DataDirectoryPath ??
                             AppDomain.CurrentDomain.BaseDirectory;

        return new FileSystemImpl(options);
    }
}
