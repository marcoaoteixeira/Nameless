using System.Reflection;
using Nameless.FileProvider.Common;
using MS_Path = System.IO.Path;
using MS_FileProvider = Microsoft.Extensions.FileProviders.EmbeddedFileProvider;

namespace Nameless.FileProvider.Embedded {
    public sealed class EmbeddedFileProvider : IFileProvider {
        #region Private Read-Only Fields

        private readonly MS_FileProvider _provider;

        #endregion

        #region Public Constructors

        public EmbeddedFileProvider (Assembly assembly) {
            Prevent.ParameterNull (assembly, nameof (assembly));

            _provider = new MS_FileProvider (assembly, assembly.GetName ().Name);
        }

        #endregion

        #region Private Static Methods

        private static string AssertPath (string path) {
            var result = path.Replace (MS_Path.DirectorySeparatorChar, '.').Replace (MS_Path.AltDirectorySeparatorChar, '.').TrimStart ('.');

            // We are replacing the hyphen with a dash
            // because the way that the C# compiler stores
            // the files inside the assembly.
            // See: https://github.com/aspnet/FileSystem/issues/184
            result = result.Replace ("-", "_");

            return result;
        }

        #endregion

        #region IFileProvider Members

        public FileProviderType Type => FileProviderType.Embedded;

        public IDirectory GetDirectory (string path) {
            Prevent.ParameterNull (path, nameof (path));

            var newPath = AssertPath (path);
            var directory = new Directory (newPath, _provider.GetDirectoryContents (newPath));

            return directory;
        }

        public IFile GetFile (string path) {
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));

            var newPath = AssertPath (path);
            var file = new File (_provider.GetFileInfo (newPath));

            return file;
        }

        public IWatchToken Watch (string filter) {
            return NullWatchToken.Instance;
        }

        #endregion
    }
}