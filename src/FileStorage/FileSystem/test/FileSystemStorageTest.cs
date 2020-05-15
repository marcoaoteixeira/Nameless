using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nameless.FileStorage.FileSystem.Test {
    public class FileSystemStorageTest : FileSystemStorageFixture {

        [Fact]
        public async Task CreateDirectory_Test () {
            // set root path to the test assembly running location.
            var directoryName = Guid.NewGuid ().ToString ("N");

            // instantiate the file system storage
            var fileStorage = new FileSystemStorage (new FileStorageSettings {
                Root = Root
            });

            // retrieve the directory.
            var directory = await fileStorage.CreateDirectoryAsync (directoryName);
            var directoryExists = System.IO.Directory.Exists (Path.Combine (Root, directoryName));

            // assert
            Assert.True (directory);
            Assert.True (directoryExists);
        }

        [Fact]
        public async Task GetDirectory_Test () {
            // set root path to the test assembly running location.
            var directoryName = Guid.NewGuid ().ToString ("N");
            var directoryPath = Path.Combine (Root, directoryName);

            // Ensure directory exists
            System.IO.Directory.CreateDirectory (directoryPath);

            // instantiate the file system storage
            var fileStorage = new FileSystemStorage (new FileStorageSettings {
                Root = Root
            });

            // retrieve the directory.
            var directory = await fileStorage.GetDirectoryAsync (directoryName);

            // assert
            Assert.NotNull (directory);
            Assert.Equal (directoryName, directory.Name);
            Assert.True (directory.Exists);
        }

        [Fact]
        public async Task GetRootDirectory_Test () {
            // instantiate the file system storage
            var fileStorage = new FileSystemStorage (new FileStorageSettings {
                Root = Root
            });

            // retrieve the directory.
            var directory = await fileStorage.GetDirectoryAsync (string.Empty);

            // assert
            Assert.NotNull (directory);
            Assert.Equal (new DirectoryInfo (Root).Name, directory.Name);
            Assert.True (directory.Exists);
        }

        [Fact]
        public async Task CreateFileAsync_Test () {
            // instantiate the file system storage
            var fileStorage = new FileSystemStorage (new FileStorageSettings {
                Root = Root
            });

            var text = "This is a test";
            var buffer = Encoding.UTF8.GetBytes (text);
            using var memoryStream = new MemoryStream (buffer);

            var fileName = $"{Guid.NewGuid ():N}.txt";
            await fileStorage.CreateFileAsync (fileName, memoryStream, overwrite : true);

            var filePath = Path.Combine (Root, fileName);
            var file = new FileInfo (filePath);

            // assert
            Assert.NotNull (file);
            Assert.True (file.Exists);
            using var streamReader = file.OpenText ();
            Assert.Equal (text, streamReader.ReadToEnd ());
        }
    }
}