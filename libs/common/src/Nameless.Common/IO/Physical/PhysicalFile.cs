using System.Diagnostics;

namespace Nameless.IO.Physical;

/// <summary>
///     Default implementation of <see cref="IFile"/>.
/// </summary>
[DebuggerDisplay(value: "{RelativePath,nq}")]
public class PhysicalFile : IFile {
    private string RelativePath => SysPath.GetRelativePath(_manager.Root, _file.FullName);

    private readonly FileInfo _file;
    private readonly PhysicalFileManager _manager;

    /// <inheritdoc />
    public string Name => _file.Name;

    /// <inheritdoc />
    public string Path => _file.FullName;

    /// <inheritdoc />
    public bool Exists => _file.Exists;

    /// <inheritdoc />
    public long Length => _file.Length;

    /// <inheritdoc />
    public DateTimeOffset LastWriteTime => _file.LastWriteTimeUtc;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="PhysicalFile"/> class.
    /// </summary>
    /// <param name="file">
    ///     The underlying <see cref="FileInfo"/> object.
    /// </param>
    /// <param name="manager">
    ///     The file manager.
    /// </param>
    public PhysicalFile(FileInfo file, PhysicalFileManager manager) {
        _file = file;
        _manager = manager;
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
        var path = _manager.GetFullPath(
            destinationRelativePath
        );

        var file = _file.CopyTo(path, overwrite);
        
        return new PhysicalFile(file, _manager);
    }
}