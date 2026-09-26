using System.Diagnostics;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Nameless.IO.Embedded;

/// <summary>
///     Read-only implementation of <see cref="IDirectory"/> for a directory
///     of files embedded as resources into an assembly.
/// </summary>
[DebuggerDisplay(value: "{Path,nq}")]
public class EmbeddedDirectory : IDirectory {
    private readonly string _relativePath;
    private readonly EmbeddedFileProvider _provider;

    /// <inheritdoc />
    /// <remarks>
    ///     For the root directory, returns the assembly name.
    /// </remarks>
    public string Name => _relativePath.Length == 0
        ? _provider.AssemblyName
        : EmbeddedPathUtils.GetName(_relativePath);

    /// <inheritdoc />
    /// <remarks>
    ///     Format: <c>embedded://{AssemblyName}/{RelativePath}</c>.
    /// </remarks>
    public string Path => $"{_provider.Root}{_relativePath}";

    /// <inheritdoc />
    public bool Exists => _provider.Manifest.GetDirectoryContents(ToManifestPath(_relativePath)).Exists;

    internal EmbeddedDirectory(string relativePath, EmbeddedFileProvider provider) {
        _relativePath = relativePath;
        _provider = provider;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     always, directories cannot be created into the assembly.
    /// </exception>
    public void Create() {
        throw new InvalidOperationException("Embedded directories cannot be created.");
    }

    /// <inheritdoc />
    /// <remarks>
    ///     The match is case-insensitive, same as the embedded
    ///     files manifest lookups.
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="glob"/> is empty or white space.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="glob"/> is <see langword="null"/>.
    /// </exception>
    public IEnumerable<IFile> GetFiles(string glob) {
        Throws.When.NullOrWhiteSpace(glob);

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase).AddInclude(glob);
        var result = matcher.Match(EnumerateFiles(_relativePath, prefix: string.Empty));

        foreach (var match in result.Files) {
            yield return new EmbeddedFile(
                EmbeddedPathUtils.Combine(_relativePath, match.Path),
                _provider
            );
        }
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     always, embedded directories cannot be deleted.
    /// </exception>
    public void Delete(bool recursive) {
        throw new InvalidOperationException("Embedded directories cannot be deleted.");
    }

    // Returns the paths of all files underneath the directory,
    // relative to this directory.
    private IEnumerable<string> EnumerateFiles(string directory, string prefix) {
        foreach (var entry in _provider.Manifest.GetDirectoryContents(ToManifestPath(directory))) {
            var path = EmbeddedPathUtils.Combine(prefix, entry.Name);

            if (!entry.IsDirectory) {
                yield return path;

                continue;
            }

            foreach (var file in EnumerateFiles(EmbeddedPathUtils.Combine(directory, entry.Name), path)) {
                yield return file;
            }
        }
    }

    // the manifest does not resolve an empty path as its root
    private static string ToManifestPath(string relativePath) {
        return relativePath.Length == 0
            ? EmbeddedPathUtils.SEPARATOR.ToString()
            : relativePath;
    }
}
