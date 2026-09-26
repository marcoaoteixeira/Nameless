using System.Buffers;
using Microsoft.Extensions.Primitives;

namespace Nameless.IO.System;

internal static class PathUtils {
    private static readonly char[] PathSeparators = [
        SysPath.AltDirectorySeparatorChar,
        SysPath.DirectorySeparatorChar
    ];

    private static readonly SearchValues<char> InvalidPathChars = SearchValues.Create(
        [.. SysPath.GetInvalidFileNameChars()
                   .Where(@char => @char != SysPath.AltDirectorySeparatorChar &&
                                   @char != SysPath.DirectorySeparatorChar)]
    );

    internal static bool HasInvalidPathChars(string path) {
        return path.AsSpan().ContainsAny(InvalidPathChars);
    }

    internal static bool PathNavigatesAboveRoot(string path) {
        var tokenizer = new StringTokenizer(path, PathSeparators);
        var depth = 0;

        foreach (var segment in tokenizer) {
            if (segment.Equals(".") || segment.Equals(string.Empty)) {
                continue;
            }
            
            if (segment.Equals("..")) {
                depth--;

                if (depth == -1) {
                    return true;
                }
            }
            else { depth++; }
        }

        return false;
    }

    internal static string EnsureTrailingSlash(string path) {
        return !string.IsNullOrEmpty(path) &&
               path[^1] != SysPath.DirectorySeparatorChar &&
               path[^1] != SysPath.AltDirectorySeparatorChar
            ? $"{path}{SysPath.DirectorySeparatorChar}"
            : path;
    }
}
