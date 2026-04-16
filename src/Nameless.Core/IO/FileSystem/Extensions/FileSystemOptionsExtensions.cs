using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

/// <summary>
///     <see cref="FileSystemOptions"/> extension methods.
/// </summary>
public static class FileSystemOptionsExtensions {
    /// <param name="self">
    ///     The current <see cref="FileSystemOptions"/>.    
    /// </param>
    extension(FileSystemOptions self) {
        /// <summary>
        ///     Ensure the given path is within the root directory.
        /// </summary>
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
        public void EnsureRootDirectory(string path) {
            if (self.AllowOperationOutsideRoot) { return; }

            path = PathHelper.Normalize(path);

            if (!path.StartsWith(self.Root, StringComparison.Ordinal)) {
                throw new UnauthorizedAccessException(message: "The specified path is outside the root directory.");
            }
        }
    }
}