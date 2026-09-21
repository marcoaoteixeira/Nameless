namespace Nameless.IO.System;

public class FileTests : IDisposable {
    private readonly string _root;

    public FileTests() {
        _root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (SysDirectory.Exists(_root)) {
            SysDirectory.Delete(_root, recursive: true);
        }
    }

    private FileProviderOptions CreateOptions() {
        return new FileProviderOptions {
            Root = _root,
            AllowOperationOutsideRoot = false
        };
    }

    private File CreateWrapper(string fileName) {
        return new File(
            new FileInfo(
                SysPath.Combine(_root, fileName)
            ),
            CreateOptions()
        );
    }

    private string CreateTempFile(string fileName, string content = "test") {
        var path = SysPath.Combine(_root, fileName);

        SysFile.WriteAllText(path, content);
        
        return path;
    }

    [Fact]
    [IntegrationTest]
    public void Exists_NonexistentFile_ReturnsFalse() {
        // arrange
        var sut = CreateWrapper("does-not-exist.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    [IntegrationTest]
    public void Exists_ExistingFile_ReturnsTrue() {
        // arrange
        CreateTempFile("exists.txt");
        var sut = CreateWrapper("exists.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    [IntegrationTest]
    public void Name_ReturnsFileName() {
        // arrange
        const string FileName = "named-file.txt";
        var sut = CreateWrapper(FileName);

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal(FileName, actual);
    }

    [Fact]
    [IntegrationTest]
    public void Path_ReturnsFullPath() {
        // arrange
        const string FileName = "path-file.txt";
        var expected = SysPath.Combine(_root, FileName);
        var sut = CreateWrapper(FileName);

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    [IntegrationTest]
    public void Delete_ExistingFile_RemovesFile() {
        // arrange
        var filePath = CreateTempFile("to-delete.txt");
        var sut = CreateWrapper("to-delete.txt");

        // act
        sut.Delete();

        // assert
        Assert.False(SysFile.Exists(filePath));
    }

    [Fact]
    [IntegrationTest]
    public void Open_ExistingFile_ReturnsReadableStream() {
        // arrange
        const string Content = "hello world";
        CreateTempFile("readable.txt", Content);
        var sut = CreateWrapper("readable.txt");

        // act
        using var stream = sut.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        var actual = reader.ReadToEnd();

        // assert
        Assert.Equal(Content, actual);
    }

    [Fact]
    public void LastWriteTime_ReturnsValidUtcTime() {
        // arrange
        CreateTempFile("last-write.txt");
        var sut = CreateWrapper("last-write.txt");

        // act
        var actual = sut.LastWriteTime;

        // assert — the write time should be a plausible recent UTC timestamp
        Assert.Multiple(
            () => Assert.True(actual > DateTime.UtcNow.AddMinutes(-5)),
            () => Assert.Equal(DateTimeKind.Utc, actual.Kind)
        );
    }

    [Fact]
    public void Copy_WithOverwriteFalse_CreatesFileAtDestination() {
        // arrange
        const string SourceFile = "copy-source.txt";
        const string DestFile = "copy-dest.txt";

        CreateTempFile(SourceFile, "copy content");
        var sut = CreateWrapper(SourceFile);

        // act
        var copy = sut.Copy(DestFile, overwrite: false);

        // assert
        Assert.Multiple(
            () => Assert.True(copy.Exists),
            () => Assert.Equal(SysPath.Combine(_root, DestFile), copy.Path)
        );
    }

    [Fact]
    public void Copy_WithOverwriteTrue_OverwritesExistingFile() {
        // arrange
        const string SourceFile = "over-source.txt";
        const string DestFile = "over-dest.txt";

        CreateTempFile(SourceFile, "new content");
        CreateTempFile(DestFile, "old content");
        var sut = CreateWrapper(SourceFile);

        // act
        var copy = sut.Copy(DestFile, overwrite: true);

        // assert — the file at dest now contains the source content
        using var stream = copy.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        var actual = reader.ReadToEnd();

        Assert.Equal("new content", actual);
    }

    [Fact]
    public void Copy_ReturnsFileWrapper_WithCorrectName() {
        // arrange
        const string SourceFile = "nm-source.txt";
        const string DestFile = "nm-dest.txt";

        CreateTempFile(SourceFile);
        var sut = CreateWrapper(SourceFile);

        // act
        var copy = sut.Copy(DestFile, overwrite: false);

        // assert
        Assert.Equal(DestFile, copy.Name);
    }
}
