using System.Collections.Frozen;

namespace Nameless.Compression.Internals;

/// <summary>
///     Device names Windows reserves in every directory (<c>CON</c>,
///     <c>NUL</c>, <c>COM1</c>, ...). Writing to such a path talks to a
///     device instead of creating a file. The name is reserved regardless
///     of case, extension (<c>nul.txt</c>) or trailing spaces (
///     <c>CON .txt</c>).
/// </summary>
internal static class WindowsReservedNames {
    private static readonly char[] Separators = [
        SysPath.DirectorySeparatorChar,
        SysPath.AltDirectorySeparatorChar
    ];

    private static FrozenSet<string> Names { get; } = new[] {
        "CON",
        "PRN",
        "AUX",
        "NUL",
        "CONIN$",
        "CONOUT$"
    }.Concat(Enumerable.Range(0, 10).SelectMany(idx => new[] { $"COM{idx}", $"LPT{idx}" }))
     .Concat([
         "COM¹",
         "COM²",
         "COM³",
         "LPT¹",
         "LPT²",
         "LPT³"
     ])
     .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Whether a single path segment (file or directory name) is reserved.
    /// </summary>
    public static bool IsReserved(string pathSegment) {
        var dot = pathSegment.IndexOf('.');
        var stem = (dot < 0 ? pathSegment : pathSegment[..dot]).TrimEnd(' ');

        return Names.Contains(stem);
    }

    /// <summary>
    ///     Whether any segment of an archive entry name (<c>/</c> or <c>\</c>
    ///     separated) is reserved.
    /// </summary>
    public static bool ContainsReservedSegment(string entryName) {
        return entryName.Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                        .Any(IsReserved);
    }
}
