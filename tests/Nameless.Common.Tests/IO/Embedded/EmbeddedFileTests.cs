using System.Reflection;

namespace Nameless.IO.Embedded;

[UnitTest]
public class EmbeddedFileTests {
    private const string Resources = "IO/Embedded/Resources";

    private static readonly Assembly TestAssembly = typeof(EmbeddedFileTests).Assembly;

    private readonly EmbeddedFileProvider _provider = new(TestAssembly);

    private static string ReadAll(Stream stream) {
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    // --- Properties ---

    [Fact]
    public void Name_ReturnsFileName() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/folder-with-dash/nested.txt");

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal("nested.txt", actual);
    }

    [Fact]
    public void Path_ReturnsRootCombinedWithRelativePath() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/folder-with-dash/nested.txt");

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal($"{_provider.Root}{Resources}/folder-with-dash/nested.txt", actual);
    }

    [Fact]
    public void Exists_WhenFileIsEmbedded_ReturnsTrue() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/data.json");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void Exists_WhenFileIsNotEmbedded_ReturnsFalse() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/missing.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void Exists_WhenPathIsDirectory_ReturnsFalse() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/folder-with-dash");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void Exists_IsCaseInsensitive() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/ROOT.TXT");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void LastWriteTime_ReturnsAssemblyCreationTimeInUtc() {
        // arrange
        var expected = SysFile.GetCreationTimeUtc(TestAssembly.Location);
        var sut = _provider.GetFile($"{Resources}/root.txt");

        // act
        var actual = sut.LastWriteTime;

        // assert
        Assert.Multiple(
            () => Assert.Equal(expected, actual),
            () => Assert.Equal(DateTimeKind.Utc, actual.Kind)
        );
    }

    // --- Open ---

    [Theory]
    [InlineData(FileMode.Open, FileAccess.Read, FileShare.Read)]
    [InlineData(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)]
    [InlineData(FileMode.Create, FileAccess.Write, FileShare.None)]
    [InlineData(FileMode.Append, FileAccess.Write, FileShare.Delete)]
    public void Open_IgnoresModeAccessAndShare_ReturnsResourceContent(FileMode mode, FileAccess access, FileShare share) {
        // arrange
        var sut = _provider.GetFile($"{Resources}/folder-with-dash/deeper/deep.txt");

        // act
        using var stream = sut.Open(mode, access, share);
        var actual = ReadAll(stream);

        // assert
        Assert.Equal("deep content", actual);
    }

    [Fact]
    public void Open_ReturnsReadOnlyStream() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/root.txt");

        // act
        using var stream = sut.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        // assert
        Assert.Multiple(
            () => Assert.False(stream.CanWrite),
            () => Assert.Throws<NotSupportedException>(() => stream.WriteByte(1))
        );
    }

    [Fact]
    public void Open_WhenFileIsNotEmbedded_ThrowsFileNotFoundException() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/missing.txt");

        // act & assert
        Assert.Throws<FileNotFoundException>(() => sut.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    [Fact]
    public void Open_WhenPathIsDirectory_ThrowsFileNotFoundException() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/folder-with-dash");

        // act & assert
        Assert.Throws<FileNotFoundException>(() => sut.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    // --- Unsupported operations ---

    [Fact]
    public void Delete_ThrowsInvalidOperationException() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/root.txt");

        // act & assert
        Assert.Throws<InvalidOperationException>(sut.Delete);
    }

    [Fact]
    public void Copy_ThrowsInvalidOperationException() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/root.txt");

        // act & assert
        Assert.Throws<InvalidOperationException>(() => sut.Copy($"{Resources}/copy.txt", overwrite: true));
    }

    [Fact]
    public void Monitor_ThrowsInvalidOperationException() {
        // arrange
        var sut = _provider.GetFile($"{Resources}/root.txt");

        // act & assert
        Assert.Throws<InvalidOperationException>(sut.Monitor);
    }
}
