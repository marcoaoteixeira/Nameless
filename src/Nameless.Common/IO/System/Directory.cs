using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.IO.System;

/// <summary>
///     Default implementation of <see cref="IDirectory"/>.
/// </summary>
[DebuggerDisplay(value: "{DebuggerDisplayValue,nq}")]
public class Directory : IDirectory {
    private readonly DirectoryInfo _directory;
    private readonly FileProviderOptions _options;

    [ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Trivial)]

    private string DebuggerDisplayValue => $"Path: {SysPath.GetRelativePath(_options.Root, Path)}";
    
    /// <inheritdoc />
    public string Name => _directory.Name;

    /// <inheritdoc />
    public string Path => _directory.GetFullPath();

    /// <inheritdoc />
    public bool Exists => _directory.Exists;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="Directory"/> class.
    /// </summary>
    /// <param name="directory">
    ///     The underlying <see cref="DirectoryInfo"/> object.
    /// </param>
    /// <param name="options">
    ///     The options for configuring the file system.
    /// </param>
    public Directory(DirectoryInfo directory, FileProviderOptions options) {
        Throws.When.OutsideRootDirectory(
            directory.FullName,
            options.Root,
            ignore: options.AllowOperationOutsideRoot
        );

        _directory = directory;
        _options = options.Validate();
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
            yield return new File(file, _options);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IDirectory> GetDirectories(string searchPattern, bool recursive) {
        var searchOptions = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var directories = _directory.EnumerateDirectories(searchPattern, searchOptions);

        foreach (var directory in directories) {
            yield return new Directory(directory, _options);
        }
    }

    /// <inheritdoc />
    public void Delete(bool recursive) {
        _directory.Delete(recursive);
    }
}