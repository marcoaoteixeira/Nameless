using System.Diagnostics;
using Nameless.IO.Monitoring;

namespace Nameless.IO.System;

/// <summary>
///     Default implementation of <see cref="IFile"/>.
/// </summary>
[DebuggerDisplay(value: "{Path,nq}")]
public class File : IFile {
    private readonly FileInfo _file;
    private readonly FileProvider _provider;

    /// <inheritdoc />
    public string Name => _file.Name;

    /// <inheritdoc />
    public string Path => SysPath.GetRelativePath(_provider.Root, _file.FullName);

    /// <inheritdoc />
    public bool Exists => _file.Exists;

    /// <inheritdoc />
    public DateTime LastWriteTime => _file.LastWriteTimeUtc;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="File"/> class.
    /// </summary>
    /// <param name="file">
    ///     The underlying <see cref="FileInfo"/> object.
    /// </param>
    /// <param name="provider">
    ///     The file provider.
    /// </param>
    public File(FileInfo file, FileProvider provider) {
        _file = file;
        _provider = provider;
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
    /// <remarks>
    ///     The caller must register handlers, call
    ///     <see cref="IFileMonitor.Start" /> and dispose it.
    /// </remarks>
    public IFileMonitor Monitor() {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IFile Copy(string destinationRelativePath, bool overwrite) {
        var copy = _provider.GetFile(destinationRelativePath);

        _ = _file.CopyTo(copy.Path, overwrite);

        return copy;
    }
}