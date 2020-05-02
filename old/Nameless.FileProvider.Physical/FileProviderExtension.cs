using System.Globalization;
using System.IO;

namespace Nameless.FileProvider.Physical {
    public static class FileProviderExtension {
        #region Public Static Methods

        public static IFile GetFile (this IFileProvider source, string path, string culture) {
            Prevent.ParameterNullOrWhiteSpace (culture, nameof (culture));

            return GetFile (source, path, new CultureInfo (culture));
        }

        /// <summary>
        /// Gets the specific file by its culture.
        /// </summary>
        /// <param name="source">The physical file provider instance.</param>
        /// <param name="path">The common path to the file.</param>
        /// <param name="culture">The culture you're looking for.</param>
        /// <returns>An instance of <see cref="IFile" /></returns>
        /// <remarks>
        /// If you want to get the specified culture file, the file has in its name
        /// the culture you're looking for, eg: Path/To/The/File.{Culture}.ext
        /// If the culture is not specified, then, the path to the file will be:
        /// Path/To/The/File.ext
        /// </remarks>
        public static IFile GetFile (this IFileProvider source, string path, CultureInfo culture) {
            if (source == null) { return null; }

            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));
            Prevent.ParameterNull (culture, nameof (culture));

            var assertPath = AssertPath (path, culture);

            return source.GetFile (assertPath);
        }

        public static IWatchToken Watch (this IFileProvider source, string filter, string culture) {
            Prevent.ParameterNullOrWhiteSpace (culture, nameof (culture));

            return Watch (source, filter, new CultureInfo (culture));
        }

        public static IWatchToken Watch (this IFileProvider source, string filter, CultureInfo culture) {
            if (source == null) { return null; }

            Prevent.ParameterNullOrWhiteSpace (filter, nameof (filter));
            Prevent.ParameterNull (culture, nameof (culture));

            var assertFilter = AssertPath (filter, culture);

            return source.Watch (assertFilter);
        }

        #endregion

        #region Private Static Methods

        private static string AssertPath (string path, CultureInfo culture) {
            var currentFolderPath = Path.GetDirectoryName (path);
            var currentFileNameWithoutExtension = Path.GetFileNameWithoutExtension (path);
            var currentFileExtension = Path.GetExtension (path);
            var cultureName = culture != CultureInfo.InvariantCulture ? culture.Name : string.Empty;
            var resourcePath = !string.IsNullOrWhiteSpace (cultureName) ?
                string.Concat (currentFolderPath, Path.DirectorySeparatorChar, currentFileNameWithoutExtension, $".{culture.Name}", currentFileExtension) :
                string.Concat (currentFolderPath, Path.DirectorySeparatorChar, currentFileNameWithoutExtension, currentFileExtension);

            return resourcePath;
        }

        #endregion
    }
}