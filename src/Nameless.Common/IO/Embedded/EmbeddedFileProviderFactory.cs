using System.Collections.Concurrent;
using System.Reflection;

namespace Nameless.IO.Embedded;

/// <summary>
///     Factory of <see cref="EmbeddedFileProvider" />
/// </summary>
public class EmbeddedFileProviderFactory {
    private readonly ConcurrentDictionary<Assembly, EmbeddedFileProvider> _cache = new();

    /// <summary>
    ///     Gets or creates a new <see cref="EmbeddedFileProvider"/>.
    /// </summary>
    /// <param name="assembly">
    ///     The assembly containing the embedded files.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="EmbeddedFileProvider"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="assembly"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="assembly"/> does not contain the embedded
    ///     files manifest.
    /// </exception>
    public EmbeddedFileProvider GetOrCreate(Assembly assembly) {
        Throws.When.Null(assembly);

        return _cache.GetOrAdd(assembly, key => new EmbeddedFileProvider(key));
    }
}
