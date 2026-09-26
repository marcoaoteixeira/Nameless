namespace Nameless.IO.System;

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

        Root = PathUtils.EnsureTrailingSlash(
            SysPath.GetFullPath(root)
        );
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is empty, white space,
    ///     path is absolute, has invalid path chars or is absolute.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     if <paramref name="relativePath"/> navigates above
    ///     file provider root path or not underneath.
    /// </exception>
    public IDirectory GetDirectory(string relativePath) {
        var directory = new DirectoryInfo(
            path: GetFullPath(relativePath)
        );

        return new Directory(directory, this);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is empty, white space,
    ///     path is absolute, has invalid path chars or is absolute.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     if <paramref name="relativePath"/> navigates above
    ///     file provider root path or not underneath.
    /// </exception>
    public IFile GetFile(string relativePath) {
        var file = new FileInfo(
            fileName: GetFullPath(relativePath)
        );

        return new File(file, this);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is empty, white space,
    ///     path is absolute, has invalid path chars or is absolute.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     if <paramref name="relativePath"/> navigates above
    ///     file provider root path or not underneath.
    /// </exception>
    public string GetFullPath(string relativePath) {
        Throws.When.NullOrWhiteSpace(relativePath);
        Throws.When.HasInvalidPathChars(relativePath);
        Throws.When.PathIsRooted(relativePath);
        Throws.When.PathNavigatesAboveRoot(relativePath);

        var result = SysPath.GetFullPath(
            SysPath.Combine(Root, relativePath)
        );

        // a path resolving to the root itself has no trailing separator
        Throws.When.PathNotUnderneathRoot(PathUtils.EnsureTrailingSlash(result), Root);

        return result;
    }
}