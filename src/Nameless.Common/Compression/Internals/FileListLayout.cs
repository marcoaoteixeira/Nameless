using Nameless.IO;

namespace Nameless.Compression.Internals;

/// <summary>
///     Computes archive entry names for an arbitrary list of files, possibly
///     spread across drives and network shares.
/// </summary>
/// <remarks>
///     Each entry is the file's full path minus its root, so no drive letter
///     or server/share name ever reaches the archive: <c>C:\work\a.txt</c>
///     → <c>work/a.txt</c>, <c>\\server\share\data\b.txt</c> →
///     <c>data/b.txt</c>. Two files from different roots can therefore
///     collide; that is reported as an <see cref="ArgumentException"/>.
/// </remarks>
internal static class FileListLayout {
    private const string PARAMETER_NAME = "filePaths";

    public sealed record Entry(string SourcePath, string EntryName);

    /// <param name="paths">
    ///     Fully qualified, distinct file paths (at least one).
    /// </param>
    /// <param name="comparison">
    ///     How names are compared (case sensitivity of the file system).
    /// </param>
    /// <returns>
    ///     One entry per input path, in input order.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Two files would be stored under the same name, or a file name
    ///     clashes with a directory of another entry.
    /// </exception>
    public static IReadOnlyList<Entry> Create(IReadOnlyList<string> paths, StringComparison comparison) {
        var entries = paths.Select(ParsedPath.Parse)
                           .Select(file => new Entry(file.Source, string.Join(Separators.ForwardSlash, file.Directories.Append(file.FileName))))
                           .ToArray();

        EnsureNoClashes(entries, comparison);

        return entries;
    }

    private static void EnsureNoClashes(Entry[] entries, StringComparison comparison) {
        var comparer = StringComparer.FromComparison(comparison);
        var files = new Dictionary<string, Entry>(comparer);
        var directories = new Dictionary<string, Entry>(comparer);

        foreach (var entry in entries) {
            if (!files.TryAdd(entry.EntryName, entry)) {
                throw Clash(entry, files[entry.EntryName], entry.EntryName);
            }

            for (var separator = entry.EntryName.IndexOf(Separators.ForwardSlash); separator >= 0; separator = entry.EntryName.IndexOf(Separators.ForwardSlash, separator + 1)) {
                directories.TryAdd(entry.EntryName[..separator], entry);
            }
        }

        foreach (var (name, entry) in files) {
            if (directories.TryGetValue(name, out var other)) {
                throw Clash(entry, other, name);
            }
        }
    }

    private static ArgumentException Clash(Entry entry, Entry other, string name) {
        return new ArgumentException(
            $"'{entry.SourcePath}' and '{other.SourcePath}' would both be stored as '{name}' in the archive.",
            PARAMETER_NAME
        );
    }

    private sealed record ParsedPath(string Source, string[] Directories, string FileName) {
        public static ParsedPath Parse(string path) {
            var root = SysPath.GetPathRoot(path) ?? string.Empty;
            var directory = SysPath.GetDirectoryName(path) ?? root;
            var directories = directory[Math.Min(root.Length, directory.Length)..].Split(
                PathHelper.PathSeparators,
                StringSplitOptions.RemoveEmptyEntries
            );

            return new ParsedPath(path, directories, SysPath.GetFileName(path));
        }
    }
}
