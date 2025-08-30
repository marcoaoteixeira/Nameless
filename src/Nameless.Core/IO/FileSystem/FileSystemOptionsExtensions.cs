using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

/// <summary>
///     <see cref="FileSystemOptions"/> extension methods.
/// </summary>
public static class FileSystemOptionsExtensions {
    /// <summary>
    ///     Ensure the given path is within the root directory.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="FileSystemOptions"/>.    
    /// </param>
    /// <param name="path">
    ///     The path to check.
    /// </param>
    /// <remarks>
    ///     if <see cref="FileSystemOptions.AllowOperationOutsideRoot"/> is
    ///     <see langword="true"/>, the check is skipped.
    /// </remarks>
    /// <exception cref="UnauthorizedAccessException">
    ///     if the given path is outside the root directory.
    /// </exception>
    public static void EnsureRootDirectory(this FileSystemOptions self, string path) {
        if (self.AllowOperationOutsideRoot) { return; }

        path = PathHelper.Normalize(path);

        if (!path.StartsWith(self.Root, StringComparison.Ordinal)) {
            throw new UnauthorizedAccessException("The specified path is outside the root directory.");
        }
    }
}
