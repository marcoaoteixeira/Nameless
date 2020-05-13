using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Nameless.FileStorage.FileSystem.Test {
    public class FileSystemStorageTest {
        private static readonly string FILE_STORAGE_ROOT = typeof (FileSystemStorageTest).Assembly.GetDirectoryPath ();

        [Fact]
        public async Task GetDirectory_Test () {
            // set root path to the test assembly running location.
            var rootPath = typeof (FileSystemStorageTest).Assembly.GetDirectoryPath ();
            var directoryPath = Path.Combine (rootPath, "Resources");

            // Ensure directory exists
            System.IO.Directory.CreateDirectory (directoryPath);

            // instantiate the file system storage
            var fileStorage = new FileSystemStorage (new FileStorageSettings {
                Root = rootPath
            });

            // retrieve the directory.
            var directory = await fileStorage.GetDirectoryAsync ("Resources");

            // assert
            Assert.NotNull (directory);
            Assert.Equal ("Resources", directory.Path);
            Assert.True (directory.Exists);
        }
    }
}