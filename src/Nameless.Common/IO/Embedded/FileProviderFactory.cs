using System.Collections.Concurrent;
using System.Reflection;

namespace Nameless.IO.Embedded;

/// <summary>
///     Factory of <see cref="FileProvider" />
/// </summary>
public class FileProviderFactory {
    private readonly ConcurrentDictionary<CacheKey, FileProvider> _cache = new();

    /// <summary>
    ///     Gets or creates a new <see cref="FileProvider"/>.
    /// </summary>
    /// <remarks>
    ///     Providers are cached per assembly and root. Equivalent roots
    ///     (e.g. <c>IO/Embedded</c>, <c>./IO/Embedded/</c> or
    ///     <c>io\embedded</c>) share the same provider.
    /// </remarks>
    /// <param name="assembly">
    ///     The assembly where the resources are located.
    /// </param>
    /// <param name="root">
    ///     The root path.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="FileProvider"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="assembly"/> or
    ///     <paramref name="root"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="root"/> is absolute or
    ///     has one or more invalid path chars.
    /// </exception>
    /// <exception cref="RelativePathException">
    ///     If unable to resolve <paramref name="root"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="assembly"/> does not contain the embedded
    ///     files manifest.
    /// </exception>
    public FileProvider GetOrCreate(Assembly assembly, string root = ".") {
        Throws.When.Null(assembly);
        Throws.When.Null(root);

        var key = new CacheKey(assembly, FileProvider.ResolveRelativePathCore(root));

        return _cache.GetOrAdd(
            key: key,
            valueFactory: static (key, root) => new FileProvider(key.Assembly, root),
            factoryArgument: root
        );
    }

    // Embedded files manifest lookups are case-insensitive,
    // so roots are compared the same way.
    private readonly record struct CacheKey(Assembly Assembly, string Root) {
        public bool Equals(CacheKey other) {
            return Assembly == other.Assembly &&
                   string.Equals(Root, other.Root, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Assembly, StringComparer.OrdinalIgnoreCase.GetHashCode(Root));
        }
    }
}
