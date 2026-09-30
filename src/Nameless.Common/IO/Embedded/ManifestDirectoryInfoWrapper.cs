using System.Collections;
using Microsoft.Extensions.FileProviders;

namespace Nameless.IO.Embedded;

/// <summary>
///     ManifestDirectoryInfo Wrapper
/// </summary>
public sealed class ManifestDirectoryInfoWrapper : IDirectoryContents {
    private readonly IDirectoryContents _directory;

    /// <summary>
    ///     Gets the directory name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the directory relative path.
    /// </summary>
    public string RelativePath { get; }

    /// <summary>
    ///     Gets the directory "physical" path.
    /// </summary>
    public string PhysicalPath { get; set; }

    /// <inheritdoc />
    public bool Exists => _directory.Exists;

    private ManifestDirectoryInfoWrapper(string relativePath, string physicalPath, IDirectoryContents directory) {
        Name = Unsafe.GetManifestDirectoryName(directory) ?? string.Empty;
        RelativePath = relativePath;
        PhysicalPath = physicalPath;

        _directory = directory;
    }

    /// <inheritdoc />
    public IEnumerator<IFileInfo> GetEnumerator() {
        return _directory.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    internal static ManifestDirectoryInfoWrapper Create(string relativePath, string physicalPath, IDirectoryContents directory) {
        return new ManifestDirectoryInfoWrapper(relativePath, physicalPath, directory);
    }
}