namespace Nameless.Compression.Internals;

/// <summary>
///     Point-in-time listing of a directory tree that ignores symbolic links
///     and junctions below the root.
/// </summary>
/// <remarks>
///     Links are detected with <see cref="FileSystemInfo.LinkTarget"/> rather
///     than <see cref="FileAttributes.ReparsePoint"/>: cloud placeholders
///     (OneDrive) and deduplicated files are reparse points too, but they are
///     real files. The tree is walked manually so link targets are never
///     entered, which also rules out link loops.
/// </remarks>
internal sealed record DirectorySnapshot {
    private static readonly EnumerationOptions TopLevelEverything = new() { AttributesToSkip = 0 };

    /// <summary>
    ///     Full paths of every regular file.
    /// </summary>
    public IReadOnlyList<string> Files { get; init; }

    /// <summary>
    ///     Full paths of directories with no (non-link) children, including
    ///     the root when empty.
    /// </summary>
    public IReadOnlyList<string> EmptyDirectories { get; init; }

    /// <summary>
    ///     Full paths of the links that were ignored.
    /// </summary>
    public IReadOnlyList<string> SkippedLinks { get; init; }

    /// <summary>
    ///     Initializes a new instance of <see cref="DirectorySnapshot"/>
    ///     class.
    /// </summary>
    /// <param name="files">
    ///     Full paths of every regular file.
    /// </param>
    /// <param name="emptyDirectories">
    ///     Full paths of directories with no (non-link) children, including
    ///     the root when empty.
    /// </param>
    /// <param name="skippedLinks">
    ///     Full paths of the links that were ignored.
    /// </param>
    public DirectorySnapshot(IReadOnlyList<string> files, IReadOnlyList<string> emptyDirectories, IReadOnlyList<string> skippedLinks) {
        Files = files;
        EmptyDirectories = emptyDirectories;
        SkippedLinks = skippedLinks;
    }
    
    public static DirectorySnapshot Take(string root, CancellationToken cancellationToken) {
        var files = new List<string>();
        var emptyDirectories = new List<string>();
        var skippedLinks = new List<string>();
        var pending = new Stack<DirectoryInfo>();

        pending.Push(new DirectoryInfo(root));

        while (pending.TryPop(out var directory)) {
            cancellationToken.ThrowIfCancellationRequested();

            var hasChildren = false;

            foreach (var info in directory.EnumerateFileSystemInfos("*", TopLevelEverything)) {
                if (info.LinkTarget is not null) {
                    skippedLinks.Add(info.FullName);

                    continue;
                }

                hasChildren = true;

                if (info is DirectoryInfo subdirectory) { pending.Push(subdirectory); }
                else { files.Add(info.FullName); }
            }

            if (!hasChildren) {
                emptyDirectories.Add(directory.FullName);
            }
        }

        return new DirectorySnapshot(files, emptyDirectories, skippedLinks);
    }
}
