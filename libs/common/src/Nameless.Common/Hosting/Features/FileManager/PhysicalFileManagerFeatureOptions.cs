using Nameless.Contracts;
using Nameless.IO;
using Nameless.IO.Physical;

namespace Nameless.Hosting.Features.FileManager;

/// <summary>
///     Physical File Manager Feature Options
/// </summary>
public class PhysicalFileManagerFeatureOptions {
    private readonly Dictionary<string, string> _roots = new(Defaults.PathComparer);

    /// <summary>
    ///     Gets the defined roots for <see cref="PhysicalFileManager"/>.
    /// </summary>
    public IReadOnlyDictionary<string, string> Roots => _roots;

    /// <summary>
    ///     Registers a <see cref="PhysicalFileManager"/> for a specific root path.
    /// </summary>
    /// <param name="name">
    ///     The name to use in the registration. Must be used in resolution.
    /// </param>
    /// <param name="path">
    ///     The root path. Must be absolute.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="PhysicalFileManagerFeatureOptions"/> so
    ///     other actions can be chained.
    /// </returns>
    public PhysicalFileManagerFeatureOptions WithRootFor(string name, string path) {
        Throws.When.NullOrWhiteSpace(name);
        Throws.When.NoMatchingPattern(name, "^[A-Za-z0-9_]+$", ignoreCase: true);
        Throws.When.NullOrWhiteSpace(path);
        Throws.When.PathIsNotRooted(path);

        return !_roots.TryAdd(name, path)
            ? throw new InvalidOperationException($"Root for '{name}' already defined.")
            : this;
    }
}