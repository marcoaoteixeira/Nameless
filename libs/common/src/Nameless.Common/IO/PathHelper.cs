using Nameless.Contracts;
using System.Text.RegularExpressions;

namespace Nameless.IO;

/// <summary>
///     Path helper methods.
/// </summary>
public static class PathHelper {
    private static readonly char[] InvalidPathChars = [
        .. SysPath.GetInvalidFileNameChars()
                  .Where(@char => @char != SysPath.DirectorySeparatorChar &&
                                  @char != SysPath.AltDirectorySeparatorChar)
    ];

    private static readonly Regex InvalidPathCharsRegex = new(
        pattern: $"[{Regex.Escape(new string(InvalidPathChars))}]",
        options: RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromSeconds(1)
    );

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
}
