using Nameless.FileProvider.Common;
using MS_FileProvider = Microsoft.Extensions.FileProviders.PhysicalFileProvider;

namespace Nameless.FileProvider.Physical {
    public sealed class PhysicalFileProvider : IFileProvider {
        #region Private Read-Only Fields

        private readonly MS_FileProvider _provider;

        #endregion

        #region Public Constructors

        public PhysicalFileProvider (string root) {
            Prevent.ParameterNull (root, nameof (root));

            _provider = new MS_FileProvider (root);
        }

        #endregion

        #region IFileProvider Members

        public FileProviderType Type => FileProviderType.FileSystem;

        public IDirectory GetDirectory (string path) {
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));

            var directory = _provider.GetDirectoryContents (path);

            return new Directory (path, directory);
        }

        public IFile GetFile (string path) {
            Prevent.ParameterNullOrWhiteSpace (path, nameof (path));

            var file = _provider.GetFileInfo (path);

            return new File (file);
        }

        public IWatchToken Watch (string filter) {
            Prevent.ParameterNullOrWhiteSpace (filter, nameof (filter));

            var token = _provider.Watch (filter);

            return new WatchToken (token);
        }

        #endregion
    }
}