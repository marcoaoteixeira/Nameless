using Moq;

namespace Nameless.IO;

[UnitTest]
public class DirectoryExtensionsTests {
    // --- IsEmpty ---

    [Fact]
    public void IsEmpty_WhenNoFiles_ReturnsTrue() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(directory => directory.GetFiles("*"))
            .Returns([]);

        // act
        var actual = mock.Object.IsEmpty;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void IsEmpty_WhenHasFiles_ReturnsFalse() {
        // arrange
        var mock = new Mock<IDirectory>();
        mock.Setup(directory => directory.GetFiles("*"))
            .Returns([Mock.Of<IFile>()]);

        // act
        var actual = mock.Object.IsEmpty;

        // assert
        Assert.False(actual);
    }

    // --- GetFiles() ---

    [Fact]
    public void GetFiles_WithoutArgs_UsesRecursiveMatchAllGlob() {
        // arrange
        IFile[] expected = [Mock.Of<IFile>(), Mock.Of<IFile>()];
        var mock = new Mock<IDirectory>();
        mock.Setup(directory => directory.GetFiles("**/*"))
            .Returns(expected);

        // act
        var actual = mock.Object.GetFiles();

        // assert
        Assert.Multiple(
            () => Assert.Same(expected, actual),
            () => mock.Verify(directory => directory.GetFiles("**/*"), Times.Once)
        );
    }

    // --- Delete ---

    [Fact]
    public void Delete_WithoutArgs_CallsDeleteNonRecursive() {
        // arrange
        var mock = new Mock<IDirectory>();

        // act
        mock.Object.Delete();

        // assert
        mock.Verify(directory => directory.Delete(false), Times.Once);
    }
}
