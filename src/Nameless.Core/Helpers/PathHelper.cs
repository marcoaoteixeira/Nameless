namespace Nameless.Helpers;

/// <summary>
///     Helper to deal with path related problems.
/// </summary>
public static class PathHelper {
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
        return path.Replace(Separators.FORWARD_SLASH[0], Path.DirectorySeparatorChar)
                   .Replace(Separators.BACKWARD_SLASH[0], Path.DirectorySeparatorChar);
    }
}