using System;
using System.IO;

namespace Nameless.Helpers {
    public static class PathHelper {
        #region Public Static Methods

        /// <summary>
        /// Normalizes a path using the path delimiter semantics of the abstract virtual file storage.
        /// </summary>
        /// <remarks>
        /// Backslash is converted to forward slash and any leading or trailing slashes
        /// are removed.
        /// </remarks>
        public static string NormalizePath (string path) {
            if (path == null) { return null; }

            return path.Replace (Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim (Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <param name="allowOutsideFileSystem"></param>
        /// <returns></returns>
        public static string GetPhysicalPath (string root, string path, bool allowOutsideFileSystem = false) {
            path = NormalizePath (path);

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