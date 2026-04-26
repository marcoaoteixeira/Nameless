using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

/// <summary>
///     <see cref="FileSystemProviderOptions"/> extension methods.
/// </summary>
public static class FileSystemProviderOptionsExtensions {
    /// <param name="self">
    ///     The current <see cref="FileSystemProviderOptions"/>.    
    /// </param>
    extension(FileSystemProviderOptions self) {
        /// <summary>
        ///     Ensure the given path is within the root directory.
        /// </summary>
        /// <param name="path">
        ///     The path to check.
        /// </param>
        /// <remarks>
        ///     if <see cref="FileSystemProviderOptions.AllowOperationOutsideRoot"/> is
        ///     <see langword="true"/>, the check is skipped.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        ///     if the given path is outside the root directory.
        /// </exception>
        public void EnsureRootDirectory(string path) {
            if (self.AllowOperationOutsideRoot) { return; }

            path = PathHelper.Normalize(path);

            if (!path.StartsWith(self.Root, StringComparison.Ordinal)) {
                throw new UnauthorizedAccessException(
                    "The specified path is outside the root directory."
                );
            }
        }
    }
}