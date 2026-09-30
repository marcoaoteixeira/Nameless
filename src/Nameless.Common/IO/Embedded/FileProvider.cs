using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Nameless.IO.Embedded;

/// <summary>
///     Read-only implementation of <see cref="IFileProvider"/> for files
///     embedded as resources into an assembly.
/// </summary>
/// <remarks>
///     <para>
///         The assembly must reference the
///         <c>Microsoft.Extensions.FileProviders.Embedded</c> package directly
///         and set <c>&lt;GenerateEmbeddedFilesManifest&gt;true&lt;/GenerateEmbeddedFilesManifest&gt;</c>
///         in its project file, so the embedded files manifest (which keeps
///         the original folder structure and file names) is generated.
///     </para>
///     <para>
///         Paths are relative to the project root of the assembly, separated
///         by <c>/</c> and case-insensitive.
///     </para>
/// </remarks>
public class FileProvider : IFileProvider {
    private const string MANIFEST_ROOT = "/";
    private const char FORWARD_SLASH = '/';

    private readonly ManifestEmbeddedFileProvider _provider;

    /// <inheritdoc />
    /// <remarks>
    ///     Format: <c>embedded://{AssemblyName}/{Root}/</c>, always ending
    ///     with <c>/</c>. For the manifest root it is
    ///     <c>embedded://{AssemblyName}/</c>.
    /// </remarks>
    public string Root { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="FileProvider"/> class.
    /// </summary>
    /// <param name="assembly">
    ///     The assembly containing the embedded files.
    /// </param>
    /// <param name="root">
    ///     The relative path from the root of the manifest to use as root
    ///     for the provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="assembly"/> or
    ///     <paramref name="root"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="root"/> is absolute or
    ///     has one or more invalid path chars.
    /// </exception>
    /// <exception cref="RelativePathException">
    ///     If <paramref name="root"/> escapes the assembly
    ///     root.
    /// </exception>
    public FileProvider(Assembly assembly, string root = ".") {
        Throws.When.Null(root);
        Throws.When.Null(assembly);
        Throws.When.PathIsRooted(root);
        Throws.When.PathHasInvalidChars(root);
        Throws.When.PathNavigatesAboveRoot(root);

        root = ResolveRelativePathCore(root);

        _provider = new ManifestEmbeddedFileProvider(
            assembly,
            root.Length > 0 ? root : MANIFEST_ROOT,
            assembly.LastWriteTimeUtc.UtcDateTime
        );

        Root = root.Length > 0
            ? $"embedded://{assembly.GetName().Name}/{root}"
            : $"embedded://{assembly.GetName().Name}";
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is empty, white space,
    ///     has invalid path chars or is absolute.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     if <paramref name="relativePath"/> navigates above
    ///     file provider root path.
    /// </exception>
    public IFile GetFile(string relativePath) {
        var path = GetFullPath(relativePath);
        var file = _provider.GetFileInfo(relativePath) switch {
            { } result => result,
            _ => throw new FileNotFoundException(
                message: $"Unable to locate file '{relativePath}' in '{Root}'",
                fileName: SysPath.GetFileName(relativePath)
            )
        };

        return new File(
            ManifestFileInfoWrapper.Create(path, file)
        );
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    ///     if <paramref name="relativePath"/> is empty, white space,
    ///     has invalid path chars or is absolute.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///     if <paramref name="relativePath"/> navigates above
    ///     file provider root path.
    /// </exception>
    public IDirectory GetDirectory(string relativePath) {
        var directory = _provider.GetDirectoryContents(relativePath) switch {
            { } result => result,
            _ => throw new DirectoryNotFoundException(
                message: $"Unable to locate directory '{relativePath}' in '{Root}'"
            )
        };

        return new Directory(
            ManifestDirectoryInfoWrapper.Create(
                relativePath: relativePath,
                physicalPath: GetFullPath(relativePath),
                directory: directory
            ),
            this
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
    public string GetFullPath(string relativePath) {
        Throws.When.Null(relativePath);
        Throws.When.PathIsRooted(relativePath);
        Throws.When.PathHasInvalidChars(relativePath);
        Throws.When.PathNavigatesAboveRoot(relativePath);

        relativePath = PathHelper.ResolveRelativePath(
            relativePath,
            separator: SeparatorType.ForwardSlash
        );

        var path = !string.IsNullOrWhiteSpace(relativePath)
            ? $"{Root}/{relativePath}"
            : Root;

        // Defense in depth: Throws.When.PathNavigatesAboveRoot already
        // rejects traversal segments, but the final path must never
        // leave the root.
        return Throws.When.PathUnderneathRoot(Root, relativePath, path);
    }
    
    /// <summary>
    ///     Resolves the provider relative path to its canonical form: segments
    ///     separated by <c>/</c>, without leading or trailing separator.
    ///     The root paths resolves to <see cref="string.Empty"/>.
    /// </summary>
    internal static string ResolveRelativePathCore(string relativePath) {
        return PathHelper.ResolveRelativePath(
            relativePath,
            separator: SeparatorType.ForwardSlash
        );
    }
}