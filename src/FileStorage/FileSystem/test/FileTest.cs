using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Nameless.FileStorage.FileSystem.Test {
    public class FileTest {
        [Fact]
        public async Task Watch_Test () {
            // arrange
            var rootPath = typeof (FileSystemStorageTest).Assembly.GetDirectoryPath ();
            var resourcesPath = Path.Combine (rootPath, "Resources");
            System.IO.Directory.CreateDirectory (resourcesPath);
            var filePath = Path.Combine (resourcesPath, "test.txt");
            System.IO.File.WriteAllText (filePath, "TEST");
            var fileStorage = new FileSystemStorage (new FileStorageSettings {
                Root = rootPath
            });
            var changed = false;

            // act
            var file = await fileStorage.GetFileAsync ("Resources\tests.txt");
            file.Watch (state => {
                changed = true;
            });

            System.IO.File.WriteAllText (filePath, "TESTtest");
            await Task.Delay (2000);

            // assert
            Assert.NotNull (file);
            Assert.True (changed);
        }
    }
}