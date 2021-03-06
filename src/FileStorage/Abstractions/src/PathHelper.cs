using System;
using System.IO;

namespace Nameless.FileStorage {
    public static class PathHelper {

        #region Internal Static Methods

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
        public static string Normalize (string path) {
            if (path == null) { return null; }

            return path.Replace (Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Trim (Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Retrieves the physical path to a file or directory.
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
        public static string GetPhysicalPath (string root, string relativePath, bool allowOutsideFileSystem = false) {
            root = Normalize (root);
            root = !root.EndsWith (Path.DirectorySeparatorChar) ? string.Concat (root, Path.DirectorySeparatorChar) : root;
            relativePath = Normalize (relativePath);

            var result = string.IsNullOrWhiteSpace (relativePath) ? root : Path.Combine (root, relativePath);

            if (!allowOutsideFileSystem) {
                // Verify that the resulting path is inside the root file system path.
                var isInsideFileSystem = Path.GetFullPath (result).StartsWith (root, StringComparison.OrdinalIgnoreCase);
                if (!isInsideFileSystem) {
                    throw new PathResolutionException ($"The path '{relativePath}' resolves to a physical path outside the file system store root.");
                }
            }

            return result;
        }

        #endregion
    }
}