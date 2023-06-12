namespace Nameless.FileStorage.System {

    public static class PathHelper {

        #region Public Static Methods

        /// <summary>
        /// Normalizes a path using the path delimiter semantics of the
        /// underlying OS platform.
        /// </summary>
        /// <remarks>
        /// <para>
        /// On Windows: Foward slash is converted to backslash and any leading
        /// or trailing slashes are removed.
        /// </para>
        /// <para>
        /// On Linux and OSX: Backslash is converted to foward slash and any
        /// leading or trailing slashes are removed.
        /// </para>
        /// </remarks>
        public static string Normalize(string path) {
            Prevent.NullOrWhiteSpaces(path, nameof(path));

            var result = path
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                .Trim(Path.DirectorySeparatorChar);

            return result;
        }

        /// <summary>
        /// Retrieves the physical path to a file.
        /// It also executes the <see cref="Normalize(string)"/> method.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="relativePath">
        /// The relative path to the <paramref name="root" />.
        /// </param>
        /// <param name="allowOutsideFileSystem">
        /// <c>true</c> if allow search outside the root; otherwise
        /// <c>false</c>.
        /// </param>
        /// <returns>The physical path to the content.</returns>
        public static string GetPhysicalPath(string root, string relativePath) {
            Prevent.NullOrWhiteSpaces(root, nameof(root));
            Prevent.NullOrWhiteSpaces(relativePath, nameof(relativePath));

            root = Normalize(root);
            relativePath = Normalize(relativePath);
            var result = Path.Join(root, relativePath);

            // Verify that the resulting path is inside the root file system path.
            var isInsideFileSystem = Path.GetFullPath(result).StartsWith(root, StringComparison.OrdinalIgnoreCase);
            if (!isInsideFileSystem) {
                throw new PathResolutionException($"The path '{result}' resolves to a physical path outside the file storage root.");
            }

            return result;
        }

        #endregion
    }
}