using System.Diagnostics;
using SysPath = System.IO.Path;

namespace Nameless.IO.Explorer.Wrappers;

/// <summary>
///     Default implementation of <see cref="IFile"/>.
/// </summary>
[DebuggerDisplay(value: "{DebuggerDisplayValue,nq}")]
public class FileWrapper : IFile {
    private readonly FileInfo _file;

    private string DebuggerDisplayValue => $"Path: {SysPath.GetRelativePath(Options.Root, Path)}";

    private FileExplorerOptions Options { get; }

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
    public FileWrapper(FileInfo file, FileExplorerOptions options) {
        _file = file;

        Options = options.EnsureRootDirectory(
            _file.GetFullPath()
        );
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
        var destinationFullPath = SysPath.GetFullPath(destinationRelativePath, Options.Root);

        destinationFullPath = PathHelper.Normalize(destinationFullPath);

        Options.EnsureRootDirectory(destinationFullPath);

        var copy = _file.CopyTo(destinationFullPath, overwrite);

        return new FileWrapper(copy, Options);
    }
}