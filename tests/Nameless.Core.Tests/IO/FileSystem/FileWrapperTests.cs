using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.IO.FileSystem;

[UnitTest]
public class FileWrapperTests {
    private static IOptions<FileSystemOptions> CreateOptions(string root = null, bool allowOperationOutsideRoot = false) {
        return OptionsHelper.Create<FileSystemOptions>(opts => {
            opts.Root = root ?? typeof(FileWrapperTests).Assembly.GetDirectoryPath();
            opts.AllowOperationOutsideRoot = allowOperationOutsideRoot;
        });
    }

    [Fact]
    public void WhenGettingName_ThenReturnsFileNameFromUnderlyingFile() {
        // arrange
        const string FileName = "ThisIsATest.txt";
        var path = ResourceHelper.GetFilePath(FileName);
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, CreateOptions());

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal(FileName, actual);
    }

    [Fact]
    public void WhenGettingPath_ThenReturnsFullPathFromUnderlyingFile() {
        // arrange
        const string FileName = "ThisIsATest.txt";
        var path = ResourceHelper.GetFilePath(FileName);
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, CreateOptions());

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal(path, actual);
    }

    [Fact]
    public void WhenCheckingFileExistence_WhenUnderlyingFileExists_ThenReturnsTrue() {
        // arrange
        const string FileName = "ThisIsATest.txt";
        var path = ResourceHelper.GetFilePath(FileName);
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, CreateOptions());

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void WhenCheckingFileExistence_WhenUnderlyingFileDoesNotExist_ThenReturnsFalse() {
        // arrange
        var options = CreateOptions();
        var path = Path.Combine(options.Value.Root, "ThisFileDoesNotExist.txt");
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, options);

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void WhenGettingLastWriteTime_ThenReturnsFileLastWriteTime() {
        // arrange
        const string FileName = "ThisIsATest.txt";
        var path = ResourceHelper.GetFilePath(FileName);
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, CreateOptions());

        // act
        var actual = sut.LastWriteTime;

        // assert
        Assert.NotEqual(DateTime.MinValue, actual);
    }

    [Fact]
    public void WhenOpeningTheFile_ThenReturnsStreamToTheFile() {
        // arrange
        const string FileName = "ThisIsATest.txt";
        var path = ResourceHelper.GetFilePath(FileName);
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, CreateOptions());

        // act
        using var actual = sut.Open();

        // assert
        Assert.NotEqual(Stream.Null, actual);
    }

    [Fact]
    public void WhenOpeningTheFile_WhenTheFileContentIsKnown_WhenReadTheStream_ThenReturnsTheFileContent() {
        // arrange
        const string Expected = "This Is A Test";
        const string FileName = "ThisIsATest.txt";
        var path = ResourceHelper.GetFilePath(FileName);
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, CreateOptions());

        // act
        using var stream = new StreamReader(sut.Open());
        var actual = stream.ReadToEnd();

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void WhenDeletingFile_ThenFileShouldBeDeleted() {
        // arrange
        var path = ResourceHelper.CreateCopy("ThisIsATest.txt", "0d8c7a52-11fb-4306-928c-98214d778666");
        var file = new FileInfo(path);
        var sut = new FileWrapper(file, CreateOptions());

        // act
        var exists = sut.Exists;
        sut.Delete();
        var deleted = !sut.Exists;

        // assert
        Assert.Multiple(() => {
            Assert.True(exists);
            Assert.True(deleted);
        });
    }

    [Fact]
    public void WhenCopyingFile_ThenANewFileShouldBeCreated() {
        // arrange

        const string FileName = "884c94ba-5958-47c0-b2a6-3fdffc79faf0.txt";
        const string NewFileName = "77c08319-4b92-4f93-9482-2732c67d05c7.txt";
        const string Content = nameof(WhenCopyingFile_ThenANewFileShouldBeCreated);

        // Define a root directory for the test files
        var root = Path.Combine(typeof(FileWrapperTests).Assembly.GetDirectoryPath(), "FileSystem");
        var fileWrapperDirectoryPath = Path.Combine(root, "FileWrapper");

        // Ensure the directory exists
        Directory.CreateDirectory(fileWrapperDirectoryPath);

        // Define the path for the temporary file
        var filePath = Path.Combine(fileWrapperDirectoryPath, FileName);

        // Ensure the file does not already exist
        if (File.Exists(filePath)) { File.Delete(filePath); }

        // Create a temporary file in the root directory
        File.WriteAllText(filePath, Content);

        // Create the FileWrapper instance
        var file = new FileInfo(filePath);
        var sut = new FileWrapper(file, CreateOptions(root));

        // act
        var actual = sut.Copy($@"FileWrapper\{NewFileName}", overwrite: true);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Exists);
            Assert.Equal(NewFileName, actual.Name);
        });
    }

    [Fact]
    public void WhenCopyingFile_WhenNotAllowOperationOutsideRootPath_ThenThrowsException() {
        // arrange

        const string FileName = "0042c8e3-2f0d-42e7-927e-8224eefdd21d.txt";
        const string NewFileName = "3fc1f6c5-2864-4e4c-9e5e-8b3de23d1b2c.txt";
        const string Content = nameof(WhenCopyingFile_WhenNotAllowOperationOutsideRootPath_ThenThrowsException);

        // Define a root directory for the test files
        var root = Path.Combine(typeof(FileWrapperTests).Assembly.GetDirectoryPath(), "FileSystem");
        var fileWrapperDirectoryPath = Path.Combine(root, "FileWrapper");

        // Ensure the directory exists
        Directory.CreateDirectory(fileWrapperDirectoryPath);

        // Define the path for the temporary file
        var filePath = Path.Combine(fileWrapperDirectoryPath, FileName);

        // Ensure the file does not already exist
        if (File.Exists(filePath)) { File.Delete(filePath); }

        // Create a temporary file in the root directory
        File.WriteAllText(filePath, Content);

        // Create the FileWrapper instance
        var file = new FileInfo(filePath);
        var sut = new FileWrapper(file, CreateOptions(root));

        // act
        // Attempt to copy outside the root
        var actual = Record.Exception(() => sut.Copy($@"..\..\..\{NewFileName}", overwrite: true));

        // assert
        Assert.IsType<UnauthorizedAccessException>(actual);
    }

    [Fact]
    public void WhenCopyingFile_WhenAllowOperationOutsideRootPath_ThenCopyFileOutsideRoot() {
        // arrange

        const string FileName = "e561fbeb-6156-4f2b-b36f-07721c85057b.txt";
        const string NewFileName = "432dc3b7-5975-423d-8d8e-fc467a5ca127.txt";
        const string Content = nameof(WhenCopyingFile_WhenAllowOperationOutsideRootPath_ThenCopyFileOutsideRoot);

        // Define a root directory for the test files
        var root = Path.Combine(typeof(FileWrapperTests).Assembly.GetDirectoryPath(), "FileSystem");
        var fileWrapperDirectoryPath = Path.Combine(root, "FileWrapper");

        // Ensure the directory exists
        Directory.CreateDirectory(fileWrapperDirectoryPath);

        // Define the path for the temporary file
        var filePath = Path.Combine(fileWrapperDirectoryPath, FileName);

        // Ensure the file does not already exist
        if (File.Exists(filePath)) { File.Delete(filePath); }

        // Create a temporary file in the root directory
        File.WriteAllText(filePath, Content);

        // Create the FileWrapper instance
        var file = new FileInfo(filePath);
        var sut = new FileWrapper(file, CreateOptions(root, allowOperationOutsideRoot: true));

        // act
        // Attempt to copy outside the root
        var destinationRelativePath = PathHelper.Normalize($@"..\..\..\{NewFileName}");
        var actual = sut.Copy(destinationRelativePath, overwrite: true);

        // assert
        Assert.Multiple(() => {
            Assert.True(actual.Exists);
            Assert.Equal(NewFileName, actual.Name);
            Assert.False(actual.Path.StartsWith(root));
        });
    }
}
