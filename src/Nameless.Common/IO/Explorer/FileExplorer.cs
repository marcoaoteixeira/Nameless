using Microsoft.Extensions.Options;
using Nameless.IO.Explorer.Wrappers;

namespace Nameless.IO.Explorer;

/// <summary>
///     Default implementation of <see cref="IFileExplorer"/>.
/// </summary>
public class FileExplorer : IFileExplorer {
    private FileExplorerOptions Options { get; }

    /// <inheritdoc />
    public string Root => Options.Root;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="FileExplorer"/> class.
    /// </summary>
    /// <param name="options">
    ///     The options for configuring the file system.
    /// </param>
    public FileExplorer(IOptions<FileExplorerOptions> options) {
        Options = options.Value.Validate();
    }

    /// <inheritdoc />
    public IDirectory GetDirectory(string relativePath) {
        var path = GetFullPath(relativePath);
        var directory = new DirectoryInfo(path);

        return new DirectoryWrapper(directory, Options);
    }

    /// <inheritdoc />
    public IFile GetFile(string relativePath) {
        var path = GetFullPath(relativePath);
        var file = new FileInfo(path);

        return new FileWrapper(file, Options);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     If the <paramref name="relativePath"/> is rooted, then it will
    ///     return the full path given its root. Otherwise, the path root
    ///     will be related to the current <see cref="IFileExplorer"/>.
    /// </remarks>
    public string GetFullPath(string relativePath) {
        var normalizeRelativePath = PathHelper.Normalize(relativePath);

        var path = Path.IsPathRooted(normalizeRelativePath)
            ? Path.GetFullPath(normalizeRelativePath)
            : Path.GetFullPath(normalizeRelativePath, Options.Root);

        Options.EnsureRootDirectory(path);

        return path;
    }
}