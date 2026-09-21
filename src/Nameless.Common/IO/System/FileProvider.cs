using Microsoft.Extensions.Options;

namespace Nameless.IO.System;

/// <summary>
///     Default implementation of <see cref="IFileProvider"/>.
/// </summary>
public class FileProvider : IFileProvider {
    private readonly FileProviderOptions _options;

    /// <inheritdoc />
    public string Root => _options.Root;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="FileProvider"/> class.
    /// </summary>
    /// <param name="options">
    ///     The options for configuring the file system.
    /// </param>
    public FileProvider(IOptions<FileProviderOptions> options) {
        _options = options.Value.Validate();
    }

    /// <inheritdoc />
    public IDirectory GetDirectory(string relativePath) {
        var directory = new DirectoryInfo(
            path: GetFullPath(relativePath)
        );

        return new Directory(directory, _options);
    }

    /// <inheritdoc />
    public IFile GetFile(string relativePath) {
        var file = new FileInfo(
            fileName: GetFullPath(relativePath)
        );

        return new File(file, _options);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     If the <paramref name="relativePath"/> is rooted, then it will
    ///     return the full path given its root. Otherwise, the path root
    ///     will be related to the current <see cref="IFileProvider"/>.
    /// </remarks>
    public string GetFullPath(string relativePath) {
        relativePath = PathHelper.Normalize(relativePath);

        var path = SysPath.IsPathRooted(relativePath)
            ? SysPath.GetFullPath(relativePath)
            : SysPath.GetFullPath(relativePath, _options.Root);

        return Throws.When.OutsideRootDirectory(
            fullPath: path,
            root: _options.Root,
            ignore: _options.AllowOperationOutsideRoot
        );
    }
}