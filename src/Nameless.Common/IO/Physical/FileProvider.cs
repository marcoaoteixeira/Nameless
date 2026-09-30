namespace Nameless.IO.Physical;

/// <summary>
///     Default implementation of <see cref="IFileProvider"/>.
/// </summary>
public class FileProvider : IFileProvider {
    /// <inheritdoc />
    public string Root { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="FileProvider"/> class.
    /// </summary>
    /// <param name="root">
    ///     The root path.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="root"/> is empty, white space
    ///     or path is not absolute.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="root"/> is <see langword="null"/>.
    /// </exception>
    public FileProvider(string root) {
        Throws.When.NullOrWhiteSpace(root);
        Throws.When.PathIsNotRooted(root);

        Root = PathHelper.RemoveTrailingSlash(
            SysPath.GetFullPath(
                PathHelper.Normalize(root)
            )
        );
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is absolute or
    ///     has one or more invalid path chars.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="RelativePathException">
    ///     If unable to resolve the relative path.
    /// </exception>
    public IDirectory GetDirectory(string relativePath) {
        var path = GetFullPath(relativePath);
        var directory = new DirectoryInfo(path);

        return new Directory(directory, this);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is absolute or
    ///     has one or more invalid path chars.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="RelativePathException">
    ///     If unable to resolve the relative path.
    /// </exception>
    public IFile GetFile(string relativePath) {
        var path = GetFullPath(relativePath);
        var file = new FileInfo(path);

        return new File(file, this);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is absolute or
    ///     has one or more invalid path chars.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="RelativePathException">
    ///     If unable to resolve the relative path.
    /// </exception>
    public string GetFullPath(string relativePath) {
        Throws.When.Null(relativePath);
        Throws.When.PathIsRooted(relativePath);
        Throws.When.PathHasInvalidChars(relativePath);
        Throws.When.PathNavigatesAboveRoot(relativePath);
        
        var path = SysPath.GetFullPath(
            SysPath.Combine(Root, relativePath)
        );

        // Defense in depth: Throws.When.PathNavigatesAboveRoot already
        // rejects traversal segments, but the final path must never
        // leave the root.
        return Throws.When.PathUnderneathRoot(Root, relativePath, path);
    }
}