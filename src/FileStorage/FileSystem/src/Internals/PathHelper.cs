using System;
using System.IO;
using Nameless.Helpers;

namespace Nameless.FileStorage.FileSystem {
    internal static class PathHelper {
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
        internal static string Normalize (string path) {
            if (path == null) { return null; }

            // On windows = '/'; otherwise '\'
            var delimiter = OperatingSystemHelper.IsWindows ? '\\' : '/';

            // On windows = '\'; otherwise '/'
            var replace = OperatingSystemHelper.IsWindows ? '/' : '\\';

            return path.Replace (replace, delimiter).Trim (delimiter);
        }

        /// <summary>
        /// Retrieves the physical path to a file or directory.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="path">The content path.</param>
        /// <param name="allowOutsideFileSystem">
        /// <c>true</c> if allow search outside the root; otherwise
        /// <c>false</c>.
        /// </param>
        /// <returns>The physical path to the content.</returns>
        internal static string GetPhysicalPath (string root, string path, bool allowOutsideFileSystem = false) {
            root = Normalize (root);
            path = Normalize (path);

            var result = string.IsNullOrEmpty (path) ? root : Path.Combine (root, path);

            if (!allowOutsideFileSystem) {
                // Verify that the resulting path is inside the root file system path.
                var isInsideFileSystem = Path.GetFullPath (result).StartsWith (root, StringComparison.OrdinalIgnoreCase);
                if (!isInsideFileSystem) {
                    throw new PathResolutionException ($"The path '{path}' resolves to a physical path outside the file system store root.");
                }
            }

            return result;
        }

        #endregion
    }
}