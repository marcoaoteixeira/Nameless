using System.Diagnostics;
using Microsoft.Extensions.Options;
using SysPath = System.IO.Path;

namespace Nameless.IO.FileSystem.Impl;

/// <summary>
///     Default implementation of <see cref="IDirectory"/>.
/// </summary>
[DebuggerDisplay(value: "{DebuggerDisplayValue,nq}")]
public class DirectoryWrapper : IDirectory {
    private readonly DirectoryInfo _directory;
    private readonly IOptions<FileSystemOptions> _options;

    private string DebuggerDisplayValue => $"Path: {SysPath.GetRelativePath(Options.Root, Path)}";

    private FileSystemOptions Options => _options.Value;

    /// <inheritdoc />
    public string Name => _directory.Name;

    /// <inheritdoc />
    public string Path => _directory.GetFullPath();

    /// <inheritdoc />
    public bool Exists => _directory.Exists;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="DirectoryWrapper"/> class.
    /// </summary>
    /// <param name="directory">
    ///     The underlying <see cref="DirectoryInfo"/> object.
    /// </param>
    /// <param name="options">
    ///     The options for configuring the file system.
    /// </param>
    public DirectoryWrapper(DirectoryInfo directory, IOptions<FileSystemOptions> options) {
        _directory = directory;
        _options = options;

        _options.Value.EnsureRootDirectory(Path);
    }

    /// <inheritdoc />
    public void Create() {
        _directory.Create();
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     when
    ///         <paramref name="fileName"/>
    ///     do not resolve to a file inside the current directory;
    ///     or do not resolve to an actual file.
    /// </exception>
    public IFile GetFile(string fileName) {
        var path = SysPath.GetFullPath(
            path: fileName,
            basePath: Path
        );

        if (!Path.Equals(SysPath.GetDirectoryName(path), StringComparison.OrdinalIgnoreCase)) {
            throw new InvalidOperationException("Specified file name does not resolve to a file inside the current directory.");
        }

        if (string.IsNullOrWhiteSpace(SysPath.GetFileName(path))) {
            throw new InvalidOperationException("Parameter value do not resolve to a file.");
        }

        return new FileWrapper(
            file: new FileInfo(path),
            options: _options
        );
    }

    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles(string searchPattern, bool recursive) {
        var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = _directory.EnumerateFiles(searchPattern, searchOptions);
        
        foreach (var file in files) {
            yield return new FileWrapper(file, _options);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IDirectory> GetDirectories(string searchPattern, bool recursive) {
        var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var directories = _directory.EnumerateDirectories(searchPattern, searchOptions);

        foreach (var directory in directories) {
            yield return new DirectoryWrapper(directory, _options);
        }
    }

    /// <inheritdoc />
    public void Delete(bool recursive) {
        _directory.Delete(recursive);
    }
}