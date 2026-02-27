using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Infrastructure;

namespace Nameless.IO.FileSystem;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the file system services.
        /// </summary>
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
        public IServiceCollection RegisterFileSystem(Action<FileSystemOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterFileSystem();
        }

        /// <summary>
        ///     Registers the file system services.
        /// </summary>
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
        public IServiceCollection RegisterFileSystem(IConfiguration configuration) {
            var section = configuration.GetSection<FileSystemOptions>();

            return self.Configure<FileSystemOptions>(section)
                       .InnerRegisterFileSystem();
        }

        private IServiceCollection InnerRegisterFileSystem() {
            self.TryAddSingleton<IFileSystem>(ResolveFileSystem);

            return self;
        }
    }

    private static FileSystemImpl ResolveFileSystem(IServiceProvider provider) {
        var options = provider.GetOptions<FileSystemOptions>();

        if (!string.IsNullOrWhiteSpace(options.Value.Root)) {
            return new FileSystemImpl(options);
        }

        var applicationContext = provider.GetService<IApplicationContext>();

        var rootDirectoryPath = applicationContext is not null
            ? applicationContext.DataDirectoryPath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

        options.Value.Root = Directory.CreateDirectory(rootDirectoryPath).FullName;

        return new FileSystemImpl(options);
    }
}