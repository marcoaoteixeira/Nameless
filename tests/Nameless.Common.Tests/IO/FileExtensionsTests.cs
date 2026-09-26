using Moq;

namespace Nameless.IO;

[UnitTest]
public class FileExtensionsTests {
    // --- Open ---

    [Fact]
    public void Open_WithDefaults_CallsOpenWithOpenOrCreateReadWriteAndShareReadWrite() {
        // arrange
        using var stream = new MemoryStream();
        var mock = new Mock<IFile>();
        mock.Setup(file => file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            .Returns(stream);

        // act
        var actual = mock.Object.Open();

        // assert
        Assert.Multiple(
            () => Assert.Same(stream, actual),
            () => mock.Verify(file => file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), Times.Once)
        );
    }

    [Fact]
    public void Open_WithMode_ForwardsModeAndKeepsDefaultAccessAndShare() {
        // arrange
        using var stream = new MemoryStream();
        var mock = new Mock<IFile>();
        mock.Setup(file => file.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            .Returns(stream);

        // act
        var actual = mock.Object.Open(FileMode.Create);

        // assert
        Assert.Same(stream, actual);
    }

    // --- Copy ---

    [Fact]
    public void Copy_WithDefaults_CallsCopyWithOverwriteFalse() {
        // arrange
        const string Destination = "dest/file.txt";
        var copy = Mock.Of<IFile>();
        var mock = new Mock<IFile>();
        mock.Setup(file => file.Copy(Destination, false))
            .Returns(copy);

        // act
        var actual = mock.Object.Copy(Destination);

        // assert
        Assert.Multiple(
            () => Assert.Same(copy, actual),
            () => mock.Verify(file => file.Copy(Destination, false), Times.Once)
        );
    }
}
