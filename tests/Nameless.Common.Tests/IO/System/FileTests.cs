namespace Nameless.IO.System;

[IntegrationTest]
public class FileTests : IDisposable {
    private readonly string _root;
    private readonly FileProvider _provider;

    public FileTests() {
        _root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(_root);

        _provider = new FileProvider(_root);
    }

    public void Dispose() {
        if (SysDirectory.Exists(_root)) {
            SysDirectory.Delete(_root, recursive: true);
        }
    }

    private File CreateSut(string relativePath) {
        return new File(new FileInfo(_provider.GetFullPath(relativePath)), _provider);
    }

    private string CreateFileOnDisk(string relativePath, string content = "test") {
        var path = _provider.GetFullPath(relativePath);

        SysDirectory.CreateDirectory(SysPath.GetDirectoryName(path) ?? _root);
        SysFile.WriteAllText(path, content);

        return path;
    }

    private static string ReadAll(IFile file) {
        using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    // --- Properties ---

    [Fact]
    public void Name_ReturnsFileName() {
        // arrange
        var sut = CreateSut("sub/named-file.txt");

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal("named-file.txt", actual);
    }

    [Fact]
    public void Path_ReturnsFullPath() {
        // arrange
        var sut = CreateSut("sub/path-file.txt");

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal(SysPath.Combine(_root, "sub", "path-file.txt"), actual);
    }

    [Fact]
    public void Exists_WhenFileDoesNotExist_ReturnsFalse() {
        // arrange
        var sut = CreateSut("does-not-exist.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void Exists_WhenFileExists_ReturnsTrue() {
        // arrange
        CreateFileOnDisk("exists.txt");
        var sut = CreateSut("exists.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void LastWriteTime_ReturnsLastWriteTimeInUtc() {
        // arrange
        var path = CreateFileOnDisk("last-write.txt");
        var expected = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        SysFile.SetLastWriteTimeUtc(path, expected);
        var sut = CreateSut("last-write.txt");

        // act
        var actual = sut.LastWriteTime;

        // assert
        Assert.Multiple(
            () => Assert.Equal(expected, actual),
            () => Assert.Equal(DateTimeKind.Utc, actual.Kind)
        );
    }

    // --- Open ---

    [Fact]
    public void Open_ExistingFileForReading_ReturnsStreamWithFileContent() {
        // arrange
        CreateFileOnDisk("readable.txt", "hello world");
        var sut = CreateSut("readable.txt");

        // act
        var actual = ReadAll(sut);

        // assert
        Assert.Equal("hello world", actual);
    }

    [Fact]
    public void Open_WithCreateModeForWriting_CreatesFileWithContent() {
        // arrange
        var sut = CreateSut("writable.txt");

        // act
        using (var stream = sut.Open(FileMode.Create, FileAccess.Write, FileShare.None))
        using (var writer = new StreamWriter(stream)) {
            writer.Write("written");
        }

        // assert
        Assert.Equal("written", SysFile.ReadAllText(SysPath.Combine(_root, "writable.txt")));
    }

    [Fact]
    public void Open_NonexistentFileWithOpenMode_ThrowsFileNotFoundException() {
        // arrange
        var sut = CreateSut("missing.txt");

        // act & assert
        Assert.Throws<FileNotFoundException>(() => sut.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    // --- Delete ---

    [Fact]
    public void Delete_ExistingFile_RemovesFileFromDisk() {
        // arrange
        var path = CreateFileOnDisk("to-delete.txt");
        var sut = CreateSut("to-delete.txt");

        // act
        sut.Delete();

        // assert
        Assert.False(SysFile.Exists(path));
    }

    [Fact]
    public void Delete_NonexistentFile_DoesNotThrow() {
        // arrange
        var sut = CreateSut("missing.txt");

        // act
        var exception = Record.Exception(sut.Delete);

        // assert
        Assert.Null(exception);
    }

    // --- Copy ---

    [Fact]
    public void Copy_ToNewDestination_CreatesCopyInsideProviderRoot() {
        // arrange
        CreateFileOnDisk("copy-source.txt", "copy content");
        var sut = CreateSut("copy-source.txt");

        // act
        var copy = sut.Copy("copy-dest.txt", overwrite: false);

        // assert
        Assert.Multiple(
            () => Assert.True(copy.Exists),
            () => Assert.Equal("copy-dest.txt", copy.Name),
            () => Assert.Equal(SysPath.Combine(_root, "copy-dest.txt"), copy.Path),
            () => Assert.Equal("copy content", SysFile.ReadAllText(SysPath.Combine(_root, "copy-dest.txt")))
        );
    }

    [Fact]
    public void Copy_ToNestedDestination_CreatesCopyAtRelativePath() {
        // arrange
        CreateFileOnDisk("nested-source.txt", "nested content");
        SysDirectory.CreateDirectory(SysPath.Combine(_root, "target"));
        var sut = CreateSut("nested-source.txt");

        // act
        var copy = sut.Copy("target/nested-dest.txt", overwrite: false);

        // assert
        Assert.Multiple(
            () => Assert.True(copy.Exists),
            () => Assert.Equal(SysPath.Combine(_root, "target", "nested-dest.txt"), copy.Path),
            () => Assert.Equal("nested content", SysFile.ReadAllText(SysPath.Combine(_root, "target", "nested-dest.txt")))
        );
    }

    [Fact]
    public void Copy_WithOverwriteTrue_OverwritesExistingDestination() {
        // arrange
        CreateFileOnDisk("over-source.txt", "new content");
        CreateFileOnDisk("over-dest.txt", "old content");
        var sut = CreateSut("over-source.txt");

        // act
        var copy = sut.Copy("over-dest.txt", overwrite: true);

        // assert
        Assert.Equal("new content", ReadAll(copy));
    }

    [Fact]
    public void Copy_WithOverwriteFalseAndExistingDestination_ThrowsIOException() {
        // arrange
        CreateFileOnDisk("keep-source.txt", "new content");
        var destination = CreateFileOnDisk("keep-dest.txt", "old content");
        var sut = CreateSut("keep-source.txt");

        // act
        var exception = Record.Exception(() => sut.Copy("keep-dest.txt", overwrite: false));

        // assert
        Assert.Multiple(
            () => Assert.IsType<IOException>(exception),
            () => Assert.Equal("old content", SysFile.ReadAllText(destination))
        );
    }

    [Fact]
    public void Copy_WhenSourceDoesNotExist_ThrowsFileNotFoundException() {
        // arrange
        var sut = CreateSut("missing-source.txt");

        // act & assert
        Assert.Throws<FileNotFoundException>(() => sut.Copy("dest.txt", overwrite: false));
    }

    [Fact]
    public void Copy_ToDestinationAboveRoot_ThrowsUnauthorizedAccessException() {
        // arrange
        CreateFileOnDisk("escape-source.txt");
        var sut = CreateSut("escape-source.txt");

        // act & assert
        Assert.Throws<UnauthorizedAccessException>(() => sut.Copy("../escaped.txt", overwrite: false));
    }
}
