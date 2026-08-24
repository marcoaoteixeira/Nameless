namespace Nameless.IO.Explorer;

/// <summary>
///     <see cref="FileExplorerOptions"/> extension methods.
/// </summary>
public static class FileExplorerOptionsExtensions {
    /// <param name="self">
    ///     The current <see cref="FileExplorerOptions"/>.    
    /// </param>
    extension(FileExplorerOptions self) {
        /// <summary>
        ///     Ensure the given path is within the root directory.
        /// </summary>
        /// <param name="path">
        ///     The path to check.
        /// </param>
        /// <remarks>
        ///     if <see cref="FileExplorerOptions.AllowOperationOutsideRoot"/> is
        ///     <see langword="true"/>, the check is skipped.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        ///     if the given path is outside the root directory.
        /// </exception>
        public FileExplorerOptions EnsureRootDirectory(string path) {
            if (self.AllowOperationOutsideRoot) { return self; }

            path = PathHelper.Normalize(path);

            if (!path.StartsWith(self.Root, StringComparison.Ordinal)) {
                throw new UnauthorizedAccessException(
                    "The specified path is outside the root directory."
                );
            }

            return self;
        }
    }
}