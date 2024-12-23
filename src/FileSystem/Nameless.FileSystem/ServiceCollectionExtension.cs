using Microsoft.Extensions.DependencyInjection;

namespace Nameless.FileSystem;
public static class ServiceCollectionExtension {
    /// <summary>
    /// Adds the <see cref="FileSystem"/> to the container.
    /// </summary>
    /// <param name="self">The current instance of <see cref="IServiceCollection"/>.</param>
    /// <returns>The current instance of <see cref="IServiceCollection"/>, so other actions can be chained.</returns>
    public static IServiceCollection RegisterFileSystem(this IServiceCollection self)
        => self.AddSingleton<IFileSystem, FileSystem>();
}
