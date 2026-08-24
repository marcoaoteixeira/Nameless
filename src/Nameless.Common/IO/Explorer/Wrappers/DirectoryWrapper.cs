using System.Diagnostics;
using SysPath = System.IO.Path;

namespace Nameless.IO.Explorer.Wrappers;

/// <summary>
///     Default implementation of <see cref="IDirectory"/>.
/// </summary>
[DebuggerDisplay(value: "{DebuggerDisplayValue,nq}")]
public class DirectoryWrapper : IDirectory {
    private readonly DirectoryInfo _directory;

    private string DebuggerDisplayValue => $"Path: {SysPath.GetRelativePath(Options.Root, Path)}";

    private FileExplorerOptions Options { get; }

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
    public DirectoryWrapper(DirectoryInfo directory, FileExplorerOptions options) {
        _directory = directory;
        
        Options = options.EnsureRootDirectory(
            _directory.GetFullPath()
        );
    }

    /// <inheritdoc />
    public void Create() {
        _directory.Create();
    }
    
    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles(string searchPattern, bool recursive) {
        var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = _directory.EnumerateFiles(searchPattern, searchOptions);
        
        foreach (var file in files) {
            yield return new FileWrapper(file, Options);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IDirectory> GetDirectories(string searchPattern, bool recursive) {
        var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var directories = _directory.EnumerateDirectories(searchPattern, searchOptions);

        foreach (var directory in directories) {
            yield return new DirectoryWrapper(directory, Options);
        }
    }

    /// <inheritdoc />
    public void Delete(bool recursive) {
        _directory.Delete(recursive);
    }
}