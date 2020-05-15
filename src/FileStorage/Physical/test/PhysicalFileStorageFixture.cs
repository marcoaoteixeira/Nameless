using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Nameless.FileStorage.Physical.Test {
    public abstract class PhysicalFileStorageFixture : IDisposable {
        private static readonly string RESOURCES_PATH = Path.Combine (typeof (PhysicalFileStorageFixture).Assembly.GetDirectoryPath (), "Resources");

        public string Root { get; private set; }
        public string DirectoryName {
            get { return Path.GetDirectoryName (Root); }
        }

        protected PhysicalFileStorageFixture () {
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