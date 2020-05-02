using System.Globalization;
using System.IO;

namespace Nameless.FileProvider.Embedded {
    public static class FileProviderExtension {
        #region Public Static Methods

        public static IFile GetFile (this IFileProvider source, string path, string culture) {
            Prevent.ParameterNullOrWhiteSpace (culture, nameof (culture));

            return GetFile (source, path, new CultureInfo (culture));
        }

        /// <summary>
        /// Gets the specific file by its culture.
        /// </summary>
        /// <param name="source">The embedded file provider instance.</param>
        /// <param name="path">The common path to the file.</param>
        /// <param name="culture">The culture you're looking for.</param>
        /// <returns>An instance of <see cref="IFile" /></returns>
        /// <remarks>
        /// If you want to get the specified culture file, the file MUST be embedded 
        /// in the specific culture folder, ex: Path/To/The/{Culture}/File.ext
        /// If the culture is not specified, then, the path to the file will be:
        /// Path/To/The/File.ext
        /// </remarks>
        public static IFile GetFile (this IFileProvider source, string path, CultureInfo culture) {
            if (source == null) { return null; }

            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));
            Prevent.ParameterNull (culture, nameof (culture));

            var cultureName = culture != CultureInfo.InvariantCulture ? culture.Name : string.Empty;

            // We are replacing the hyphen with a dash
            // because the way that the C# compiler stores
            // the files inside the assembly.
            // See: https://github.com/aspnet/FileSystem/issues/184
            cultureName = cultureName.Replace ("-", "_");
            path = path.Replace ("-", "_");

            var currentFolderPath = Path.GetDirectoryName (path).Replace (Path.DirectorySeparatorChar, '.').Replace (Path.AltDirectorySeparatorChar, '.').TrimStart ('.');
            var currentFileName = Path.GetFileName (path);
            var resourcePath = !string.IsNullOrWhiteSpace (cultureName) ?
                string.Concat (currentFolderPath, ".", cultureName, ".", currentFileName) :
                string.Concat (currentFolderPath, ".", currentFileName);

            return source.GetFile (resourcePath);
        }

        #endregion
    }
}