using System.Collections.Concurrent;

namespace Nameless.IO.Physical;

/// <summary>
///     Factory of <see cref="PhysicalFileManager" />
/// </summary>
public class PhysicalFileManagerFactory {
    private readonly ConcurrentDictionary<string, PhysicalFileManager> _cache = new(Defaults.PathComparer);

    /// <summary>
    ///     Gets or creates a new <see cref="PhysicalFileManager"/>.
    /// </summary>
    /// <param name="root">
    ///     The root path.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="PhysicalFileManager"/>.
    /// </returns>
    public PhysicalFileManager GetOrCreate(string root) {
        return _cache.GetOrAdd(root, _ => new PhysicalFileManager(
            root
        ));
    }
}
