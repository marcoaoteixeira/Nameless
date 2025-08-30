using System.Diagnostics;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using SysPath = System.IO.Path;

namespace Nameless.IO.FileSystem;

/// <summary>
///     Default implementation of <see cref="IFile"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public class FileWrapper : IFile {
    private readonly FileInfo _file;
    private readonly string _relativePath;
    private readonly IOptions<FileSystemOptions> _options;

    private string DebuggerDisplayValue => $"Path: {_relativePath}";

    private FileSystemOptions Options => _options.Value;

    /// <inheritdoc />
    public string Name => _file.Name;

    /// <inheritdoc />
    public string Path => _file.GetFullPath();

    /// <inheritdoc />
    public bool Exists => _file.Exists;

    /// <inheritdoc />
    public DateTime LastWriteTime => _file.LastWriteTimeUtc;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="FileWrapper"/> class.
    /// </summary>
    /// <param name="file">
    ///     The underlying <see cref="FileInfo"/> object.
    /// </param>
    /// <param name="options">
    ///     The options for configuring the file system.
    /// </param>
    public FileWrapper(FileInfo file, IOptions<FileSystemOptions> options) {
        _file = Guard.Against.Null(file);
        _options = Guard.Against.Null(options);

        _options.Value.EnsureRootDirectory(Path);

        _relativePath = SysPath.GetRelativePath(Options.Root, Path);
    }

    /// <inheritdoc />
    public Stream Open(FileMode mode, FileAccess access, FileShare share) {
        return _file.Open(mode, access, share);
    }

    /// <inheritdoc />
    public void Delete() {
        _file.Delete();
    }

    /// <inheritdoc />
    public IFile Copy(string destinationRelativePath, bool overwrite) {
        Guard.Against.NullOrWhiteSpace(destinationRelativePath);

        var destinationFullPath = SysPath.GetFullPath(destinationRelativePath, Options.Root);

        destinationFullPath = PathHelper.Normalize(destinationFullPath);

        Options.EnsureRootDirectory(destinationFullPath);

        var copy = _file.CopyTo(destinationFullPath, overwrite);

        return new FileWrapper(copy, _options);
    }
}