using System.Text.RegularExpressions;

namespace Nameless.Helpers;

/// <summary>
///     Helper to deal with path related problems.
/// </summary>
public static class PathHelper {
    private static readonly Regex InvalidPathCharsRegex = new(
        pattern: $"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}]",
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
        return path.Replace(CoreConstants.Separators.ForwardSlash[index: 0], Path.DirectorySeparatorChar)
                   .Replace(CoreConstants.Separators.BackwardSlash[index: 0], Path.DirectorySeparatorChar);
    }

    /// <summary>
    ///     Sanitizes a string for use as a file system path by replacing any
    ///     characters deemed invalid by the current OS
    ///     (see <see cref="Path.GetInvalidPathChars"/>) with a safe
    ///     substitute.
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
        if (string.IsNullOrWhiteSpace(value)) { return value; }

        if (Array.IndexOf(Path.GetInvalidPathChars(), replacement) >= 0) {
            throw new ArgumentException(
                message: $"Replacement character '{replacement}' is itself an invalid path character.",
                paramName: nameof(replacement)
            );
        }

        return InvalidPathCharsRegex.Replace(value, replacement.ToString());
    }
}