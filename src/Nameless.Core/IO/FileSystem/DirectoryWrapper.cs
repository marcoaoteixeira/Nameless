using System.Diagnostics;
using Microsoft.Extensions.Options;
using SysPath = System.IO.Path;

namespace Nameless.IO.FileSystem;

/// <summary>
///     Default implementation of <see cref="IDirectory"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public class DirectoryWrapper : IDirectory {
    private readonly DirectoryInfo _directory;
    private readonly string _relativePath;
    private readonly IOptions<FileSystemOptions> _options;

    private string DebuggerDisplayValue => $"Path: {_relativePath}";

    private FileSystemOptions Options => _options.Value;

    /// <inheritdoc />
    public string Name => _directory.Name;

    /// <inheritdoc />
    public string Path => _directory.FullName;

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
        _directory = Guard.Against.Null(directory);
        _options = Guard.Against.Null(options);

        _options.Value.EnsureRootDirectory(_directory.FullName);

        _relativePath = SysPath.GetRelativePath(Options.Root, directory.FullName);
    }

    /// <inheritdoc />
    public string Create() {
        return Directory.CreateDirectory(_directory.FullName).FullName;
    }

    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles(string searchPattern, bool recursive) {
        var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = _directory.GetFiles(searchPattern, searchOptions);

        foreach (var file in files) {
            yield return new FileWrapper(file, _options);
        }
    }

    /// <inheritdoc />
    public void Delete(bool recursive) {
        _directory.Delete(recursive);
    }
}