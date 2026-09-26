namespace Nameless.IO.Embedded;

internal static class EmbeddedPathUtils {
    internal const char SEPARATOR = '/';

    private static readonly char[] PathSeparators = ['/', '\\'];

    /// <summary>
    ///     Normalizes a relative path to the manifest format: segments
    ///     separated by <c>/</c>, without empty or <c>.</c> segments and
    ///     with <c>..</c> segments resolved.
    /// </summary>
    /// <remarks>
    ///     The path must have been checked beforehand to not navigate
    ///     above the root.
    /// </remarks>
    internal static string Normalize(string relativePath) {
        var segments = new List<string>();

        foreach (var segment in relativePath.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries)) {
            switch (segment) {
                case ".":
                    continue;

                case "..":
                    segments.RemoveAt(segments.Count - 1);
                    continue;

                default:
                    segments.Add(segment);
                    break;
            }
        }

        return string.Join(SEPARATOR, segments);
    }

    internal static string Combine(string left, string right) {
        return left.Length == 0 ? right : $"{left}{SEPARATOR}{right}";
    }

    internal static string GetName(string relativePath) {
        return relativePath[(relativePath.LastIndexOf(SEPARATOR) + 1)..];
    }
}
