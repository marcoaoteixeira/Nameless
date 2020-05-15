using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Nameless.FileStorage.FileSystem.Test {
    public abstract class FileSystemStorageFixture : IDisposable {
        private static readonly string RESOURCES_PATH = Path.Combine (typeof (FileSystemStorageFixture).Assembly.GetDirectoryPath (), "Resources");

        public string Root { get; private set; }
        public string DirectoryName {
            get { return Path.GetDirectoryName (Root); }
        }

        protected FileSystemStorageFixture () {
            var directoryName = Guid.NewGuid ().ToString ("N");

            Root = Path.Combine (RESOURCES_PATH, directoryName);
            if (!System.IO.Directory.Exists (Root)) {
                System.IO.Directory.CreateDirectory (Root);
            }
        }

        public void Dispose () {
            if (System.IO.Directory.Exists (Root)) {
                System.IO.Directory.Delete (Root, recursive : true);
            }
        }
    }
}