using System.Runtime.InteropServices;

namespace Nameless.Helpers;

/// <summary>
/// Helper to deal with path related problems.
/// </summary>
public static class PathHelper {
    private const char WINDOWS_DIRECTORY_SEPARATOR_CHAR = '\\';
    private const char NON_WINDOWS_DIRECTORY_SEPARATOR_CHAR = '/';
    private const char HOME_DIRECTORY_CHAR = '~';

    /// <summary>
    /// Normalizes a path using the path delimiter semantics of the
    /// underlying OS platform.
    /// </summary>
    /// <remarks>
    /// <para>
    /// On Windows: Forward slash is converted to backslash and any leading
    /// or trailing slashes are removed.
    /// </para>
    /// <para>
    /// On Linux and OSX: Backslash is converted to forward slash and any
    /// leading or trailing slashes are removed.
    /// </para>
    /// </remarks>
    public static string Normalize(string path) {
        if (string.IsNullOrWhiteSpace(path)) { return string.Empty; }

        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var (platformDirectorySeparatorChar, platformAltDirectorySeparatorChar) = isWindows
            ? (WINDOWS_DIRECTORY_SEPARATOR_CHAR, NON_WINDOWS_DIRECTORY_SEPARATOR_CHAR)
            : (NON_WINDOWS_DIRECTORY_SEPARATOR_CHAR, WINDOWS_DIRECTORY_SEPARATOR_CHAR);

        var result = path.Replace(platformAltDirectorySeparatorChar, platformDirectorySeparatorChar)
                         .TrimEnd(platformDirectorySeparatorChar);

        // On Windows we'll leave the path as it is.
        if (isWindows) { return result; }

        // If platform is non-Windows, we need to add a directory separator
        // at the start of the path, like c/folder/file.extension => /c/folder/file.extension
        // Except, if we are at home directory: ~
        if (!result.StartsWith(HOME_DIRECTORY_CHAR) && !result.StartsWith(platformDirectorySeparatorChar)) {
            result = $"{platformDirectorySeparatorChar}{result}";
        }
        
        return result;
    }

    /// <summary>
    /// Gets the full path based on the <paramref name="root"/> and <paramref name="relativePath"/>.
    /// </summary>
    /// <param name="root">The root path.</param>
    /// <param name="relativePath"> The relative path to the <paramref name="root" />. </param>
    /// <returns>
    /// Returns the full path if it resolves to a path inside the root; otherwise <c>null</c>.
    /// </returns>
    public static string? GetFullPath(string root, string relativePath) {
        Prevent.Argument.NullOrWhiteSpace(root);
        Prevent.Argument.NullOrWhiteSpace(relativePath);

        root = Normalize(root);
        relativePath = Normalize(relativePath);

        var result = Path.Join(root, relativePath);
        var fullPath = Path.GetFullPath(result);

        // Verify that the resulting path is inside the root file system path.
        return fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase)
            ? fullPath
            : null;
    }
}