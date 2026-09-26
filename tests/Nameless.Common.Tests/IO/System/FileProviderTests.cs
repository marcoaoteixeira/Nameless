namespace Nameless.IO.System;

[IntegrationTest]
public class FileProviderTests : IDisposable {
    private readonly string _root;

    public FileProviderTests() {
        _root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (SysDirectory.Exists(_root)) {
            SysDirectory.Delete(_root, recursive: true);
        }
    }

    private string RootWithSeparator => $"{_root}{SysPath.DirectorySeparatorChar}";

    private FileProvider CreateSut() {
        return new FileProvider(_root);
    }

    // --- Constructor ---

    [Fact]
    public void Constructor_WithNullRoot_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new FileProvider(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyOrWhiteSpaceRoot_ThrowsArgumentException(string root) {
        // act & assert
        Assert.Throws<ArgumentException>(() => new FileProvider(root));
    }

    [Fact]
    public void Constructor_WithRelativeRoot_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => new FileProvider("relative/root"));
    }

    [Fact]
    public void Constructor_WithRootWithoutTrailingSeparator_AppendsSeparator() {
        // act
        var sut = new FileProvider(_root);

        // assert
        Assert.Equal(RootWithSeparator, sut.Root);
    }

    [Fact]
    public void Constructor_WithRootWithTrailingSeparator_DoesNotDuplicateSeparator() {
        // act
        var sut = new FileProvider(RootWithSeparator);

        // assert
        Assert.Equal(RootWithSeparator, sut.Root);
    }

    [Fact]
    public void Constructor_WithNonCanonicalRoot_NormalizesToFullPath() {
        // arrange
        var root = SysPath.Combine(_root, "sub", "..");

        // act
        var sut = new FileProvider(root);

        // assert
        Assert.Equal(RootWithSeparator, sut.Root);
    }

    // --- GetFullPath ---

    [Fact]
    public void GetFullPath_WithRelativePath_ReturnsPathCombinedWithRoot() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath("file.txt");

        // assert
        Assert.Equal(SysPath.Combine(_root, "file.txt"), actual);
    }

    [Fact]
    public void GetFullPath_WithNestedPathUsingForwardSlashes_ReturnsNormalizedFullPath() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath("some/nested/file.txt");

        // assert
        Assert.Equal(SysPath.Combine(_root, "some", "nested", "file.txt"), actual);
    }

    [Theory]
    [InlineData("./file.txt")]
    [InlineData("sub/../file.txt")]
    [InlineData("sub/./other/../../file.txt")]
    public void GetFullPath_WithDotSegmentsThatStayInsideRoot_ReturnsResolvedPath(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath(relativePath);

        // assert
        Assert.Equal(SysPath.Combine(_root, "file.txt"), actual);
    }

    [Fact]
    public void GetFullPath_WithNullPath_ThrowsArgumentNullException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetFullPath(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GetFullPath_WithEmptyOrWhiteSpacePath_ThrowsArgumentException(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetFullPath(relativePath));
    }

    [Fact]
    public void GetFullPath_WithInvalidPathChars_ThrowsArgumentException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetFullPath("file\0.txt"));
    }

    [Fact]
    public void GetFullPath_WithRootedPath_ThrowsArgumentException() {
        // arrange
        var sut = CreateSut();
        var absolutePath = SysPath.Combine(_root, "file.txt");

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetFullPath(absolutePath));
    }

    [Theory]
    [InlineData("../file.txt")]
    [InlineData("..")]
    [InlineData("sub/../../file.txt")]
    [InlineData("./../file.txt")]
    public void GetFullPath_WithPathNavigatingAboveRoot_ThrowsUnauthorizedAccessException(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<UnauthorizedAccessException>(() => sut.GetFullPath(relativePath));
    }

    // --- GetFile ---

    [Fact]
    public void GetFile_WithRelativePath_ReturnsFileWithRelativePathAndName() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFile("sub/test.txt");

        // assert
        Assert.Multiple(
            () => Assert.IsType<File>(actual),
            () => Assert.Equal("test.txt", actual.Name),
            () => Assert.Equal(SysPath.Combine("sub", "test.txt"), actual.Path)
        );
    }

    [Fact]
    public void GetFile_WhenFileDoesNotExist_ReturnsFileThatDoesNotExist() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFile("missing.txt");

        // assert
        Assert.False(actual.Exists);
    }

    [Fact]
    public void GetFile_WhenFileExists_ReturnsFileThatExists() {
        // arrange
        SysFile.WriteAllText(SysPath.Combine(_root, "existing.txt"), "content");
        var sut = CreateSut();

        // act
        var actual = sut.GetFile("existing.txt");

        // assert
        Assert.True(actual.Exists);
    }

    [Fact]
    public void GetFile_WithPathNavigatingAboveRoot_ThrowsUnauthorizedAccessException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<UnauthorizedAccessException>(() => sut.GetFile("../outside.txt"));
    }

    [Fact]
    public void GetFile_WithRootedPath_ThrowsArgumentException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetFile(SysPath.Combine(_root, "file.txt")));
    }

    // --- GetDirectory ---

    [Fact]
    public void GetDirectory_WithRelativePath_ReturnsDirectoryWithRelativePathAndName() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetDirectory("parent/child");

        // assert
        Assert.Multiple(
            () => Assert.IsType<Directory>(actual),
            () => Assert.Equal("child", actual.Name),
            () => Assert.Equal(SysPath.Combine("parent", "child"), actual.Path)
        );
    }

    [Fact]
    public void GetDirectory_WhenDirectoryExists_ReturnsDirectoryThatExists() {
        // arrange
        SysDirectory.CreateDirectory(SysPath.Combine(_root, "existing"));
        var sut = CreateSut();

        // act
        var actual = sut.GetDirectory("existing");

        // assert
        Assert.True(actual.Exists);
    }

    [Fact]
    public void GetDirectory_WithPathNavigatingAboveRoot_ThrowsUnauthorizedAccessException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<UnauthorizedAccessException>(() => sut.GetDirectory("../outside"));
    }

    [Fact]
    public void GetDirectory_WithNullPath_ThrowsArgumentNullException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetDirectory(null!));
    }
}
