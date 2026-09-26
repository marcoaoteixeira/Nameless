using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Nameless.IO.System;

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
public class EmbeddedFileProvider : IFileProvider {
    private const string SCHEME = "embedded://";

    internal ManifestEmbeddedFileProvider Manifest { get; }

    internal DateTime LastWriteTime { get; }

    internal string AssemblyName { get; }

    /// <inheritdoc />
    /// <remarks>
    ///     Format: <c>embedded://{AssemblyName}/</c>.
    /// </remarks>
    public string Root { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="EmbeddedFileProvider"/> class.
    /// </summary>
    /// <param name="assembly">
    ///     The assembly containing the embedded files.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="assembly"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="assembly"/> does not contain the embedded
    ///     files manifest.
    /// </exception>
    public EmbeddedFileProvider(Assembly assembly) {
        Throws.When.Null(assembly);

        Manifest = new ManifestEmbeddedFileProvider(assembly);
        LastWriteTime = ResolveLastWriteTime(assembly);
        AssemblyName = assembly.GetName().Name ?? string.Empty;
        Root = $"{SCHEME}{AssemblyName}{EmbeddedPathUtils.SEPARATOR}";
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
        return new EmbeddedFile(Normalize(relativePath), this);
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
        return new EmbeddedDirectory(Normalize(relativePath), this);
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
    public string GetFullPath(string relativePath) {
        return $"{Root}{Normalize(relativePath)}";
    }

    private static string Normalize(string relativePath) {
        Throws.When.NullOrWhiteSpace(relativePath);
        Throws.When.HasInvalidPathChars(relativePath);
        Throws.When.PathIsRooted(relativePath);

        // backslash is a separator for embedded paths on every OS, but
        // not for the navigation guard on Unix, so unify it first.
        var path = relativePath.Replace('\\', EmbeddedPathUtils.SEPARATOR);

        Throws.When.PathNavigatesAboveRoot(path);

        return EmbeddedPathUtils.Normalize(path);
    }

    private static DateTime ResolveLastWriteTime(Assembly assembly) {
        // single-file published or in-memory assemblies have no location
        return !string.IsNullOrEmpty(assembly.Location) && SysFile.Exists(assembly.Location)
            ? SysFile.GetCreationTimeUtc(assembly.Location)
            : DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    }
}
