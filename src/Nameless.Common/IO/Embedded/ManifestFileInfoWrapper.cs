using Microsoft.Extensions.FileProviders;

namespace Nameless.IO.Embedded;

/// <summary>
///     ManifestFileInfo Wrapper
/// </summary>
public sealed class ManifestFileInfoWrapper : IFileInfo {
    private readonly IFileInfo _file;

    /// <inheritdoc />
    public bool Exists => _file.Exists;

    /// <inheritdoc />
    public long Length => _file.Length;

    /// <inheritdoc />
    public string? PhysicalPath { get; }

    /// <inheritdoc />
    public string Name => _file.Name;

    /// <inheritdoc />
    public DateTimeOffset LastModified => _file.LastModified;

    /// <inheritdoc />
    public bool IsDirectory => _file.IsDirectory;

    private ManifestFileInfoWrapper(string path, IFileInfo file) {
        _file = file;

        PhysicalPath = path;
    }

    /// <inheritdoc />
    public Stream CreateReadStream() {
        return _file.CreateReadStream();
    }

    internal static ManifestFileInfoWrapper Create(string path, IFileInfo file) {
        return new ManifestFileInfoWrapper(path, file);
    }
}