using System.Diagnostics;
using Lucene.Net.Store;
using Microsoft.Extensions.Options;
using Nameless.IO.Monitoring;

namespace Nameless.IO.System;

/// <summary>
///     Default implementation of <see cref="IFile"/>.
/// </summary>
[DebuggerDisplay(value: "{DebuggerDisplayValue,nq}")]
public class File : IFile {
    private readonly FileInfo _file;
    private readonly FileProviderOptions _options;

    private string DebuggerDisplayValue => $"Path: {SysPath.GetRelativePath(_options.Root, Path)}";

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
    ///     the <see cref="File"/> class.
    /// </summary>
    /// <param name="file">
    ///     The underlying <see cref="FileInfo"/> object.
    /// </param>
    /// <param name="options">
    ///     The options for configuring the file system.
    /// </param>
    public File(FileInfo file, FileProviderOptions options) {
        Throws.When.OutsideRootDirectory(
            file.FullName,
            options.Root,
            ignore: options.AllowOperationOutsideRoot
        );

        _file = file;
        _options = options.Validate();
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
        return new FileMonitor(
            _options.Root,
            Name,
            new FileSystemWatcherAdapter(),
            FileProbe.Instance,
            TimeProvider.System,
            _options.FileMonitorOptions
        );
    }

    /// <inheritdoc />
    public IFile Copy(string destinationRelativePath, bool overwrite) {
        Throws.When.NullOrWhiteSpace(destinationRelativePath);

        var destinationFullPath = PathHelper.Normalize(
            SysPath.GetFullPath(
                destinationRelativePath,
                _options.Root
            )
        );

        Throws.When.OutsideRootDirectory(
            destinationFullPath,
            _options.Root,
            ignore: _options.AllowOperationOutsideRoot
        );

        var copy = _file.CopyTo(
            destinationFullPath,
            overwrite
        );

        return new File(copy, _options);
    }
}