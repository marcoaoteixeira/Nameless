using System.IO;
using System.Threading.Tasks;
using Nameless.Helpers;
using MS_Directory = System.IO.Directory;
using MS_File = System.IO.File;

namespace Nameless.FileStorage.FileSystem {
    public class FileSystemStorage : IFileStorage {
        #region Private Read-Only Fields

        private readonly string _root;

        #endregion

        #region Public Constructors

        public FileSystemStorage (string root) {
            Prevent.ParameterNull (root, nameof (root));

            _root = root;
        }

        #endregion

        #region IFileStorage Members

        public Task CreateFileFromStream (string path, Stream input, bool overwrite = false) {
            var physicalPath = PathHelper.GetPhysicalPath (_root, path);

            if (MS_File.Exists (physicalPath) && !overwrite) {
                throw new FileStorageException ("Cannot create file because the destination path already exist.");
            }

            // Create directory path if it doesn't exist.
            var physicalDirectoryPath = Path.GetDirectoryName (physicalPath);
            MS_Directory.CreateDirectory (physicalDirectoryPath);

            var fileInfo = new FileInfo (physicalPath);
            using (var output = fileInfo.Create ()) {
                return input.CopyToAsync (output);
            }
        }

        public Task<IFile> GetFileAsync (string path) {
            return Task.FromResult<IFile> (new File (_root, path));
        }

        public Task<bool> TryCreateDirectoryAsync (string path) {
            var physicalPath = PathHelper.GetPhysicalPath (_root, path);

            if (MS_Directory.Exists (physicalPath)) {
                return Task.FromResult (false);
            }

            MS_Directory.CreateDirectory (physicalPath);

            return Task.FromResult (true);
        }

        public Task<IDirectory> GetDirectoryAsync (string path) {
            return Task.FromResult<IDirectory> (new Directory (_root, path));
        }

        #endregion
    }
}