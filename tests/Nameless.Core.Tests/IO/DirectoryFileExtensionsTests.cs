using Moq;
using Nameless.IO.FileSystem;

namespace Nameless.IO;

public class DirectoryFileExtensionsTests {
    // --- DirectoryExtensions.IsEmpty ---

    [Fact]
    public void IsEmpty_WhenNoFiles_ReturnsTrue() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetFiles("*", recursive: true))
            .Returns([]);

        // act & assert
        Assert.True(mock.Object.IsEmpty);
    }

    [Fact]
    public void IsEmpty_WhenHasFiles_ReturnsFalse() {
        // arrange
        var fileMock = new Mock<IFile>();
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetFiles("*", recursive: true))
            .Returns([fileMock.Object]);

        // act & assert
        Assert.False(mock.Object.IsEmpty);
    }

    // --- DirectoryExtensions.GetFiles() ---

    [Fact]
    public void GetFiles_NoArgs_CallsWithStarPatternAndRecursive() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetFiles("*", recursive: true))
            .Returns([]);

        // act
        mock.Object.GetFiles();

        // assert
        mock.Verify(d => d.GetFiles("*", recursive: true), Times.Once);
    }

    // --- DirectoryExtensions.GetFiles(pattern) ---

    [Fact]
    public void GetFiles_WithPattern_CallsWithPatternAndNotRecursive() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetFiles("*.txt", recursive: false))
            .Returns([]);

        // act
        mock.Object.GetFiles("*.txt");

        // assert
        mock.Verify(d => d.GetFiles("*.txt", recursive: false), Times.Once);
    }

    // --- DirectoryExtensions.GetDirectories() ---

    [Fact]
    public void GetDirectories_NoArgs_CallsWithStarAndRecursive() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetDirectories("*", recursive: true))
            .Returns([]);

        // act
        mock.Object.GetDirectories();

        // assert
        mock.Verify(d => d.GetDirectories("*", recursive: true), Times.Once);
    }

    // --- FileExtensions.Open ---

    [Fact]
    public void Open_WithDefaults_CallsOpenWithDefaultParameters() {
        // arrange
        var stream = new MemoryStream();
        var mock = new Mock<IFile>();
        mock.Setup(f => f.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            .Returns(stream);

        // act
        var result = mock.Object.Open();

        // assert
        Assert.Same(stream, result);
        mock.Verify(f => f.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), Times.Once);
    }

    // --- DirectoryExtensions.GetFiles(bool) ---

    [Fact]
    public void GetFiles_WithRecursiveBool_CallsWithStarAndRecursiveTrue() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetFiles("*", recursive: true))
            .Returns([]);

        // act
        mock.Object.GetFiles(recursive: true);

        // assert
        mock.Verify(d => d.GetFiles("*", recursive: true), Times.Once);
    }

    [Fact]
    public void GetFiles_WithRecursiveFalse_CallsWithStarAndRecursiveFalse() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetFiles("*", recursive: false))
            .Returns([]);

        // act
        mock.Object.GetFiles(recursive: false);

        // assert
        mock.Verify(d => d.GetFiles("*", recursive: false), Times.Once);
    }

    // --- DirectoryExtensions.GetDirectories(string) ---

    [Fact]
    public void GetDirectories_WithPattern_CallsWithPatternAndNotRecursive() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetDirectories("sub*", recursive: false))
            .Returns([]);

        // act
        mock.Object.GetDirectories("sub*");

        // assert
        mock.Verify(d => d.GetDirectories("sub*", recursive: false), Times.Once);
    }

    // --- DirectoryExtensions.GetDirectories(bool) ---

    [Fact]
    public void GetDirectories_WithRecursiveTrue_CallsWithStarAndRecursiveTrue() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(d => d.GetDirectories("*", recursive: true))
            .Returns([]);

        // act
        mock.Object.GetDirectories(recursive: true);

        // assert
        mock.Verify(d => d.GetDirectories("*", recursive: true), Times.Once);
    }

    // --- DirectoryExtensions.Delete ---

    [Fact]
    public void Delete_WithDefaultRecursive_CallsDeleteWithFalse() {
        // arrange
        var mock = new Mock<IDirectory>();

        // act
        mock.Object.Delete();

        // assert
        mock.Verify(d => d.Delete(false), Times.Once);
    }

    // --- FileExtensions.Copy ---

    [Fact]
    public void Copy_WithDefaults_CallsCopyWithOverwriteFalse() {
        // arrange
        var destPath = "/dest/file.txt";
        var copyMock = new Mock<IFile>();
        var mock = new Mock<IFile>();
        mock.Setup(f => f.Copy(destPath, overwrite: false))
            .Returns(copyMock.Object);

        // act
        var result = mock.Object.Copy(destPath);

        // assert
        Assert.Same(copyMock.Object, result);
        mock.Verify(f => f.Copy(destPath, overwrite: false), Times.Once);
    }
}
