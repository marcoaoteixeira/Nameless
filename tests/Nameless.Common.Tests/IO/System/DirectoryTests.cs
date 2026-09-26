namespace Nameless.IO.System;

[IntegrationTest]
public class DirectoryTests : IDisposable {
    private readonly string _root;
    private readonly FileProvider _provider;

    public DirectoryTests() {
        _root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(_root);

        _provider = new FileProvider(_root);
    }

    public void Dispose() {
        if (SysDirectory.Exists(_root)) {
            SysDirectory.Delete(_root, recursive: true);
        }
    }

    private Directory CreateSut(string relativePath) {
        return new Directory(new DirectoryInfo(_provider.GetFullPath(relativePath)), _provider);
    }

    private string CreateDirectoryOnDisk(string relativePath) {
        return SysDirectory.CreateDirectory(_provider.GetFullPath(relativePath)).FullName;
    }

    private void CreateFileOnDisk(string relativePath) {
        var path = _provider.GetFullPath(relativePath);

        SysDirectory.CreateDirectory(SysPath.GetDirectoryName(path) ?? _root);
        SysFile.WriteAllText(path, relativePath);
    }

    // --- Properties ---

    [Fact]
    public void Name_ReturnsDirectoryName() {
        // arrange
        var sut = CreateSut("parent/child");

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal("child", actual);
    }

    [Fact]
    public void Path_ReturnsFullPath() {
        // arrange
        var sut = CreateSut("parent/child");

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal(SysPath.Combine(_root, "parent", "child"), actual);
    }

    [Fact]
    public void Exists_WhenDirectoryDoesNotExist_ReturnsFalse() {
        // arrange
        var sut = CreateSut("nonexistent");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void Exists_WhenDirectoryExists_ReturnsTrue() {
        // arrange
        CreateDirectoryOnDisk("existing");
        var sut = CreateSut("existing");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    // --- Create ---

    [Fact]
    public void Create_WhenDirectoryDoesNotExist_CreatesDirectoryOnDisk() {
        // arrange
        var sut = CreateSut("to-create");

        // act
        sut.Create();

        // assert
        Assert.True(SysDirectory.Exists(SysPath.Combine(_root, "to-create")));
    }

    [Fact]
    public void Create_WithNestedPath_CreatesAllIntermediateDirectories() {
        // arrange
        var sut = CreateSut("a/b/c");

        // act
        sut.Create();

        // assert
        Assert.True(SysDirectory.Exists(SysPath.Combine(_root, "a", "b", "c")));
    }

    [Fact]
    public void Create_WhenDirectoryAlreadyExists_DoesNotThrow() {
        // arrange
        CreateDirectoryOnDisk("already-there");
        var sut = CreateSut("already-there");

        // act
        var exception = Record.Exception(sut.Create);

        // assert
        Assert.Null(exception);
    }

    // --- Delete ---

    [Fact]
    public void Delete_EmptyDirectoryNonRecursive_RemovesDirectory() {
        // arrange
        var path = CreateDirectoryOnDisk("to-delete");
        var sut = CreateSut("to-delete");

        // act
        sut.Delete(recursive: false);

        // assert
        Assert.False(SysDirectory.Exists(path));
    }

    [Fact]
    public void Delete_NonEmptyDirectoryRecursive_RemovesDirectoryAndContent() {
        // arrange
        CreateFileOnDisk("full/sub/file.txt");
        var sut = CreateSut("full");

        // act
        sut.Delete(recursive: true);

        // assert
        Assert.False(SysDirectory.Exists(SysPath.Combine(_root, "full")));
    }

    [Fact]
    public void Delete_NonEmptyDirectoryNonRecursive_ThrowsIOException() {
        // arrange
        CreateFileOnDisk("not-empty/file.txt");
        var sut = CreateSut("not-empty");

        // act & assert
        Assert.Throws<IOException>(() => sut.Delete(recursive: false));
    }

    [Fact]
    public void Delete_NonexistentDirectory_ThrowsDirectoryNotFoundException() {
        // arrange
        var sut = CreateSut("missing");

        // act & assert
        Assert.Throws<DirectoryNotFoundException>(() => sut.Delete(recursive: false));
    }

    // --- GetFiles ---

    [Fact]
    public void GetFiles_WithNullGlob_ThrowsArgumentNullException() {
        // arrange
        var sut = CreateSut("glob");

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetFiles(null!).ToList());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GetFiles_WithEmptyOrWhiteSpaceGlob_ThrowsArgumentException(string glob) {
        // arrange
        var sut = CreateSut("glob");

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetFiles(glob).ToList());
    }

    [Fact]
    public void GetFiles_WithTopLevelGlob_ReturnsOnlyMatchingFilesInDirectory() {
        // arrange
        CreateFileOnDisk("with-files/a.txt");
        CreateFileOnDisk("with-files/b.txt");
        CreateFileOnDisk("with-files/c.log");
        CreateFileOnDisk("with-files/sub/d.txt");
        var sut = CreateSut("with-files");

        // act
        var actual = sut.GetFiles("*.txt")
                        .Select(file => file.Path)
                        .Order()
                        .ToArray();

        // assert
        Assert.Equal([
            SysPath.Combine(_root, "with-files", "a.txt"),
            SysPath.Combine(_root, "with-files", "b.txt")
        ], actual);
    }

    [Fact]
    public void GetFiles_WithRecursiveGlob_ReturnsMatchingFilesInSubdirectories() {
        // arrange
        CreateFileOnDisk("recursive/a.txt");
        CreateFileOnDisk("recursive/sub/b.txt");
        CreateFileOnDisk("recursive/sub/deeper/c.txt");
        CreateFileOnDisk("recursive/sub/d.log");
        var sut = CreateSut("recursive");

        // act
        var actual = sut.GetFiles("**/*.txt")
                        .Select(file => file.Path)
                        .Order()
                        .ToArray();

        // assert
        Assert.Equal([
            SysPath.Combine(_root, "recursive", "a.txt"),
            SysPath.Combine(_root, "recursive", "sub", "b.txt"),
            SysPath.Combine(_root, "recursive", "sub", "deeper", "c.txt")
        ], actual);
    }

    [Fact]
    public void GetFiles_ReturnsExistingFilesWithName() {
        // arrange
        CreateFileOnDisk("single/only.txt");
        var sut = CreateSut("single");

        // act
        var actual = Assert.Single(sut.GetFiles("*"));

        // assert
        Assert.Multiple(
            () => Assert.True(actual.Exists),
            () => Assert.Equal("only.txt", actual.Name)
        );
    }

    [Fact]
    public void GetFiles_WhenNoFileMatches_ReturnsEmpty() {
        // arrange
        CreateFileOnDisk("no-match/a.log");
        var sut = CreateSut("no-match");

        // act
        var actual = sut.GetFiles("*.txt");

        // assert
        Assert.Empty(actual);
    }

    [Fact]
    public void GetFiles_WhenDirectoryDoesNotExist_ReturnsEmpty() {
        // arrange
        var sut = CreateSut("missing");

        // act
        var actual = sut.GetFiles("**/*");

        // assert
        Assert.Empty(actual);
    }
}
