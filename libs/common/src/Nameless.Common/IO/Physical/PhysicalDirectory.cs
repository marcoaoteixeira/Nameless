using Microsoft.Extensions.FileSystemGlobbing;
using Nameless.Contracts;
using System.Diagnostics;

namespace Nameless.IO.Physical;

/// <summary>
///     Default implementation of <see cref="IDirectory"/>.
/// </summary>
[DebuggerDisplay(value: "{RelativePath,nq}")]
public class PhysicalDirectory : IDirectory {
    private string RelativePath => SysPath.GetRelativePath(_manager.Root, _directory.FullName);

    private static readonly StringComparison MatcherComparison = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    private readonly DirectoryInfo _directory;
    private readonly PhysicalFileManager _manager;

    /// <inheritdoc />
    public string Name => _directory.Name;

    /// <inheritdoc />
    public string Path => _directory.FullName;

    /// <inheritdoc />
    public bool Exists => _directory.Exists;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="PhysicalDirectory"/> class.
    /// </summary>
    /// <param name="directory">
    ///     The underlying <see cref="DirectoryInfo"/> object.
    /// </param>
    /// <param name="manager">
    ///     The options for configuring the file system.
    /// </param>
    public PhysicalDirectory(DirectoryInfo directory, PhysicalFileManager manager) {
        _directory = directory;
        _manager = manager;
    }

    /// <inheritdoc />
    public void Create() {
        _directory.Create();
    }
    
    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="glob"/> is empty or white space.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="glob"/> is <see langword="null"/>.
    /// </exception>
    public IEnumerable<IFile> GetFiles(string glob) {
        Throws.When.NullOrWhiteSpace(glob);

        var matcher = new Matcher(MatcherComparison).AddInclude(glob);
        
        foreach (var file in matcher.GetResultsInFullPath(Path)) {
            yield return _manager.GetFile(
                SysPath.GetRelativePath(_manager.Root, file)
            );
        }
    }

    /// <inheritdoc />
    public void Delete(bool recursive) {
        _directory.Delete(recursive);
    }
}