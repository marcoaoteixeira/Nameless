using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace Nameless.IO;

/// <summary>
///     Helper to deal with path related problems.
/// </summary>
public static class PathHelper {
    private const char BACKWARD_SLASH_CHAR = (char)SeparatorType.BackwardSlash;
    private const char FORWARD_SLASH_CHAR = (char)SeparatorType.ForwardSlash;

    private static readonly char[] PathSeparators = [
        BACKWARD_SLASH_CHAR,
        FORWARD_SLASH_CHAR
    ];

    private static readonly char[] InvalidPathChars = [
        .. SysPath.GetInvalidFileNameChars()
                  .Where(@char => @char != BACKWARD_SLASH_CHAR &&
                                  @char != FORWARD_SLASH_CHAR)
    ];
    
    private static readonly Regex InvalidPathCharsRegex = new(
        pattern: $"[{Regex.Escape(new string(InvalidPathChars))}]",
        options: RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromSeconds(1)
    );

    /// <summary>
    ///     Normalizes a path using the path delimiter semantics of the
    ///     underlying OS platform.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         On Windows: Forward slash is converted to backslash and any leading
    ///         or trailing slashes are removed.
    ///     </para>
    ///     <para>
    ///         On Linux and OSX: Backslash is converted to forward slash and any
    ///         leading or trailing slashes are removed.
    ///     </para>
    /// </remarks>
    public static string Normalize(string path) {
        return path.Replace(
            SysPath.AltDirectorySeparatorChar,
            SysPath.DirectorySeparatorChar
        );
    }

    /// <summary>
    ///     Sanitizes a string for use as a path by replacing any
    ///     characters deemed invalid (except forward and backward slash)
    ///     by the current OS (see <see cref="Path.GetInvalidPathChars"/>)
    ///     with a safe substitute.
    /// </summary>
    /// <param name="value">
    ///     The input string to sanitize.
    /// </param>
    /// <param name="replacement">
    ///     The character used to substitute each invalid character.
    ///     Defaults to <c>'_'</c>. Must not be an invalid path character
    ///     itself.
    /// </param>
    /// <returns>
    ///     A new sanitized string, or the original <paramref name="value"/>
    ///     instance if no replacements were needed.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="replacement"/> is itself an invalid
    ///     path character.
    /// </exception>
    public static string Sanitize(string value, char replacement = '_') {
        Throws.When.Null(value);

        if (Array.IndexOf(InvalidPathChars, replacement) >= 0) {
            throw new ArgumentException(
                message: $"Replacement character '{replacement}' is itself an invalid path character.",
                paramName: nameof(replacement)
            );
        }

        return InvalidPathCharsRegex.Replace(value, $"{replacement}");
    }

    /// <summary>
    ///     Ensures slash at the beginning of the path.
    /// </summary>
    /// <param name="path">
    ///     The path.
    /// </param>
    /// <param name="separator">
    ///     Type of separator
    /// </param>
    /// <returns>
    ///     The path with leading slash.
    /// </returns>
    public static string EnsureLeadingSlash(string path, SeparatorType separator = SeparatorType.BackwardSlash) {
        var slash = (char)separator;

        if (string.IsNullOrWhiteSpace(path)) {
            return $"{slash}";
        }

        return path[0] != slash ? $"{slash}{path}" : path;
    }

    /// <summary>
    ///     Ensures slash at the end of the path.
    /// </summary>
    /// <param name="path">
    ///     The path.
    /// </param>
    /// <param name="separator">
    ///     Type of separator
    /// </param>
    /// <returns>
    ///     The path with trailing slash.
    /// </returns>
    public static string EnsureTrailingSlash(string path, SeparatorType separator = SeparatorType.BackwardSlash) {
        var slash = (char)separator;

        if (string.IsNullOrWhiteSpace(path)) {
            return $"{slash}";
        }

        return path[^1] != slash ? $"{path}{slash}" : path;
    }
    
    /// <summary>
    ///     Removes the trailing slashes from the path.
    /// </summary>
    /// <remarks>
    ///     Leading slashes are kept. If the path is a volume root
    ///     (e.g. <c>C:\</c> or <c>/</c>), it is returned unchanged,
    ///     otherwise it would turn into a relative path.
    /// </remarks>
    /// <param name="path">
    ///     The path.
    /// </param>
    /// <returns>
    ///     The path without trailing slashes.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     When <paramref name="path"/> is empty or white space.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     When <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    public static string RemoveTrailingSlash(string path) {
        Throws.When.NullOrWhiteSpace(path);

        var root = SysPath.GetPathRoot(path) ?? string.Empty;
        var result = path.TrimEnd(PathSeparators);

        return result.Length < root.Length ? root : result;
    }

    /// <summary>
    ///     Resolves a relative path.
    /// </summary>
    /// <param name="relativePath">
    ///     The relative path.
    /// </param>
    /// <param name="separator">
    ///     Type of path separator.
    /// </param>
    /// <returns>
    ///     The resolved relative path.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="relativePath"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="RelativePathException">
    ///     If <paramref name="relativePath"/> escapes root.
    /// </exception>
    public static string ResolveRelativePath(string relativePath, SeparatorType separator = SeparatorType.BackwardSlash) {
        Throws.When.Null(relativePath);

        var segments = new List<string>();
        var tokenizer = new StringTokenizer(relativePath.Trim(), PathSeparators);

        foreach (var segment in tokenizer) {
            if (segment.Equals(".") || string.IsNullOrWhiteSpace(segment.Value)) {
                continue;
            }

            if (segment.Equals("..")) {
                var segmentIndex = segments.Count - 1;

                if (segmentIndex < 0) {
                    throw new RelativePathException(
                        message: $"The path '{relativePath}' escapes root.",
                        relativePath
                    );
                }

                segments.RemoveAt(segmentIndex);
                continue;
            }

            segments.Add(segment.Value);
        }

        return string.Join((char)separator, segments);
    }

    /// <summary>
    ///     Checks if the path navigates about the root.
    /// </summary>
    /// <param name="path">
    ///     The path.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if it navigates; otherwise
    ///     <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    public static bool NavigatesAboveRoot(string path) {
        Throws.When.Null(path);

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
}