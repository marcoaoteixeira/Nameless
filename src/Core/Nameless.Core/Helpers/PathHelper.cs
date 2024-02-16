namespace Nameless.Helpers {
    public static class PathHelper {
        #region Private Constants

        private const char WINDOWS_DIRECTORY_SEPARATOR_CHAR = '\\';
        private const char UNIX_DIRECTORY_SEPARATOR_CHAR = '/';

        #endregion

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
            Guard.Against.NullOrWhiteSpace(path, nameof(path));

            var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

            var result = isWindows
                ? path.Replace(UNIX_DIRECTORY_SEPARATOR_CHAR, WINDOWS_DIRECTORY_SEPARATOR_CHAR)
                : path.Replace(WINDOWS_DIRECTORY_SEPARATOR_CHAR, UNIX_DIRECTORY_SEPARATOR_CHAR);

            // On Windows we'll leave the path as it is.
            if (isWindows) { return result; }

            // For Linux, MacOS, Android, let's "fix"
            return result.StartsWith(UNIX_DIRECTORY_SEPARATOR_CHAR)
                ? result :
                $"{UNIX_DIRECTORY_SEPARATOR_CHAR}{result}";
        }

        /// <summary>
        /// Retrieves the physical path to a file.
        /// It also executes the <see cref="Normalize(string)"/> method.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="relativePath">
        /// The relative path to the <paramref name="root" />.
        /// </param>
        /// <returns>The physical path to the content.</returns>
        public static string GetPhysicalPath(string root, string relativePath) {
            Guard.Against.NullOrWhiteSpace(root, nameof(root));
            Guard.Against.NullOrWhiteSpace(relativePath, nameof(relativePath));

            root = Normalize(root);
            relativePath = Normalize(relativePath);
            var result = Path.Join(root, relativePath);
            var fullPath = Path.GetFullPath(result);

            // Verify that the resulting path is inside the root file system path.
            var isInsideFileSystem = fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase);
            if (!isInsideFileSystem) {
                throw new PathResolutionException($"The path '{result}' resolves to a physical path outside the file storage root.");
            }

            return result;
        }

        #endregion
    }
}