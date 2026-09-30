namespace Nameless.IO.Physical;

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
        Assert.Equal(_root, sut.Root);
    }

    [Fact]
    public void Constructor_WithRootWithTrailingSeparator_RemovesTrailingSeparator() {
        // act
        var sut = new FileProvider($"{_root}{SysPath.DirectorySeparatorChar}");

        // assert
        Assert.Equal(_root, sut.Root);
    }

    [Fact]
    public void Constructor_WithVolumeRoot_KeepsVolumeRoot() {
        // arrange
        var volumeRoot = SysPath.GetPathRoot(_root)!;

        // act
        var sut = new FileProvider(volumeRoot);

        // assert
        Assert.Equal(volumeRoot, sut.Root);
    }

    [Fact]
    public void Constructor_WithUncRoot_OnWindows_KeepsLeadingSeparators() {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "UNC paths exist only on Windows.");

        // act
        var sut = new FileProvider(@"\\server\share\");

        // assert
        Assert.Equal(@"\\server\share", sut.Root);
    }

    [Fact]
    public void GetFullPath_WithVolumeRoot_ReturnsPathUnderVolumeRoot() {
        // arrange
        var volumeRoot = SysPath.GetPathRoot(_root)!;
        var sut = new FileProvider(volumeRoot);

        // act
        var actual = sut.GetFullPath("file.txt");

        // assert
        Assert.Equal(SysPath.Combine(volumeRoot, "file.txt"), actual);
    }

    [Fact]
    public void Constructor_WithNonCanonicalRoot_NormalizesToFullPath() {
        // arrange
        var root = SysPath.Combine(_root, "sub", "..");

        // act
        var sut = new FileProvider(root);

        // assert
        Assert.Equal(_root, sut.Root);
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

    [Theory]
    [InlineData(".")]
    [InlineData("./")]
    [InlineData("sub/..")]
    public void GetFullPath_WithPathResolvingToRoot_ReturnsRootPath(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath(relativePath);

        // assert
        Assert.Equal(_root, SysPath.TrimEndingDirectorySeparator(actual));
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
    public void GetFullPath_WithEmptyOrWhiteSpacePath_DoesNotThrows(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act & assert
        var ex = Record.Exception(() => sut.GetFullPath(relativePath));

        Assert.Null(ex);
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
    [ClassData<OutsideRootRelativePathTheoryData>]
    public void GetFullPath_WithPathNavigatingAboveRoot_ThrowsRelativePathException(string relativePath) {
        // arrange
        var sut = CreateSut();
        var path = SysPath.Combine(relativePath);

        // act & assert
        Assert.Throws<RelativePathException>(
            () => sut.GetFullPath(path),
            ex => ex.Message.Contains("escapes root") ? null : ex.Message
        );
    }

    // --- GetFile ---

    [Fact]
    public void GetFile_WithRelativePath_ReturnsFileWithFullPathAndName() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFile("sub/test.txt");

        // assert
        Assert.Multiple(
            () => Assert.IsType<File>(actual),
            () => Assert.Equal("test.txt", actual.Name),
            () => Assert.Equal(SysPath.Combine(_root, "sub", "test.txt"), actual.Path)
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
        var path = SysPath.Combine("..", "outside.txt");
        var sut = CreateSut();

        // act & assert
        Assert.Throws<RelativePathException>(
            () => sut.GetFile(path),
            ex => ex.Message.Contains("escapes root") ? null : ex.Message
        );
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
    public void GetDirectory_WithRelativePath_ReturnsDirectoryWithFullPathAndName() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetDirectory("parent/child");

        // assert
        Assert.Multiple(
            () => Assert.IsType<Directory>(actual),
            () => Assert.Equal("child", actual.Name),
            () => Assert.Equal(SysPath.Combine(_root, "parent", "child"), actual.Path)
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
    public void GetDirectory_WithDot_ReturnsRootDirectory() {
        // arrange
        SysFile.WriteAllText(SysPath.Combine(_root, "a.txt"), "a");
        SysDirectory.CreateDirectory(SysPath.Combine(_root, "sub"));
        SysFile.WriteAllText(SysPath.Combine(_root, "sub", "b.txt"), "b");
        var sut = CreateSut();

        // act
        var actual = sut.GetDirectory(".");
        var files = actual.GetFiles("**/*")
                          .Select(file => file.Path)
                          .Order()
                          .ToArray();

        // assert
        Assert.Multiple(
            () => Assert.True(actual.Exists),
            () => Assert.Equal(_root, SysPath.TrimEndingDirectorySeparator(actual.Path)),
            () => Assert.Equal([
                SysPath.Combine(_root, "a.txt"),
                SysPath.Combine(_root, "sub", "b.txt")
            ], files)
        );
    }

    [Fact]
    public void GetDirectory_WithPathNavigatingAboveRoot_ThrowsRelativePathException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<RelativePathException>(
            () => sut.GetDirectory(SysPath.Combine("..", "outside")),
            ex => ex.Message.Contains("escapes root") ? null : ex.Message
        );
    }

    [Fact]
    public void GetDirectory_WithNullPath_ThrowsArgumentNullException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetDirectory(null!));
    }
}

public sealed class OutsideRootRelativePathTheoryData : TheoryData<string> {
    public OutsideRootRelativePathTheoryData() {
        Add(SysPath.Combine("..", "file.txt"));
        Add("..");
        Add(SysPath.Combine("sub", "..", "..", "file.txt"));
        Add(SysPath.Combine(".", "..", "file.txt"));
        Add("../file.txt");
        Add("../../file.txt");
        Add("sub/../../file.txt");

        if (OperatingSystem.IsWindows()) {
            Add(@"sub\..\../file.txt");
        }
    }
}