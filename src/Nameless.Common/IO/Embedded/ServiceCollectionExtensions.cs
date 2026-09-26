using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.IO.Embedded;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods for Embedded File Provider.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers a keyed <see cref="IFileProvider"/> for the files
        ///     embedded into the <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly containing the embedded files.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        /// <remarks>
        ///     The service key is the assembly name. Resolve it with
        ///     <c>[FromKeyedServices("AssemblyName")] IFileProvider</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="assembly"/> is <see langword="null"/>.
        /// </exception>
        public IServiceCollection RegisterEmbeddedFileProvider(Assembly assembly) {
            Throws.When.Null(assembly);

            self.TryAddSingleton<EmbeddedFileProviderFactory>();
            self.TryAddKeyedSingleton<IFileProvider>(
                serviceKey: assembly.GetName().Name,
                implementationFactory: (provider, _) => provider.GetRequiredService<EmbeddedFileProviderFactory>()
                                                                .GetOrCreate(assembly)
            );

            return self;
        }
    }
}
