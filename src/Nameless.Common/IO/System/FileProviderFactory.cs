using System.Collections.Concurrent;

namespace Nameless.IO.System;

/// <summary>
///     Factory of <see cref="FileProvider" />
/// </summary>
public class FileProviderFactory {
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    private readonly ConcurrentDictionary<string, FileProvider> _cache = new(PathComparer);

    /// <summary>
    ///     Gets or creates a new <see cref="FileProvider"/>.
    /// </summary>
    /// <param name="root">
    ///     The root path.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="FileProvider"/>.
    /// </returns>
    public FileProvider GetOrCreate(string root) {
        return _cache.GetOrAdd(root, _ => new FileProvider(
            root
        ));
    }
}
