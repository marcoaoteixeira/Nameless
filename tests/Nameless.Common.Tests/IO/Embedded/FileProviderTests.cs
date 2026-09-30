using System.Reflection;

namespace Nameless.IO.Embedded;

[UnitTest]
public class FileProviderTests {
    private static readonly Assembly TestAssembly = typeof(FileProviderTests).Assembly;
    private static readonly string ExpectedRoot = $"embedded://{TestAssembly.GetName().Name}";

    private static FileProvider CreateSut() {
        return new FileProvider(TestAssembly);
    }

    // --- Constructor ---

    [Fact]
    public void Constructor_WithNullAssembly_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new FileProvider(null!));
    }

    [Fact]
    public void Constructor_WithAssemblyWithoutManifest_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new FileProvider(typeof(object).Assembly));
    }

    [Theory]
    [InlineData("\\")]
    [InlineData("/")]
    public void Constructor_WithRootIsRoot_ThrowsArgumentException(string root) {
        // act & assert
        Assert.Throws<ArgumentException>(
            () => new FileProvider(typeof(object).Assembly, root)
        );
    }

    [Fact]
    public void Root_ReturnsEmbeddedSchemeWithAssemblyName() {
        // act
        var actual = CreateSut().Root;

        // assert
        Assert.Equal(ExpectedRoot, actual);
    }

    [Theory]
    [InlineData("IO/Embedded")]
    [InlineData("IO/Embedded/")]
    [InlineData("./IO/Embedded")]
    [InlineData("IO/Other/../Embedded")]
    public void Root_WithCustomRoot_ReturnsRootSeparatedFromAssemblyName(string root) {
        // act
        var actual = new FileProvider(TestAssembly, root).Root;

        // assert
        Assert.Equal($"{ExpectedRoot}/IO/Embedded", actual);
    }

    [Fact]
    public void GetFullPath_WithCustomRoot_ReturnsPathUnderCustomRoot() {
        // arrange
        var sut = new FileProvider(TestAssembly, "IO/Embedded");

        // act
        var actual = sut.GetFullPath("Resources/root.txt");

        // assert
        Assert.Equal($"{ExpectedRoot}/IO/Embedded/Resources/root.txt", actual);
    }

    [Fact]
    public void GetFile_WithCustomRoot_ReturnsExistingFileUnderCustomRoot() {
        // arrange
        var sut = new FileProvider(TestAssembly, "IO/Embedded");

        // act
        var actual = sut.GetFile("Resources/root.txt");

        // assert
        Assert.Multiple(
            () => Assert.True(actual.Exists),
            () => Assert.Equal($"{ExpectedRoot}/IO/Embedded/Resources/root.txt", actual.Path)
        );
    }

    [Fact]
    public void GetDirectory_WithCustomRoot_ReturnsFilesUnderCustomRoot() {
        // arrange
        var sut = new FileProvider(TestAssembly, "IO/Embedded/Resources");

        // act
        var actual = sut.GetDirectory("folder-with-dash")
                        .GetFiles("*.txt")
                        .Select(file => file.Path)
                        .ToArray();

        // assert
        Assert.Equal([$"{ExpectedRoot}/IO/Embedded/Resources/folder-with-dash/nested.txt"], actual);
    }

    // --- GetFullPath ---

    [Fact]
    public void GetFullPath_WithRelativePath_ReturnsPathCombinedWithRoot() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath(
            SysPath.Combine("IO", "Embedded", "Resources", "root.txt")
        );

        // assert
        Assert.Equal($"{ExpectedRoot}/IO/Embedded/Resources/root.txt", actual);
    }

    [Theory]
    [InlineData(@"IO\Embedded\Resources\root.txt")]
    [InlineData("./IO/Embedded/Resources/root.txt")]
    [InlineData("IO//Embedded/Resources/root.txt")]
    [InlineData("IO/Embedded/Other/../Resources/root.txt")]
    [InlineData("IO/Embedded/Resources/root.txt/")]
    public void GetFullPath_WithNonCanonicalPath_ReturnsNormalizedPath(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath(relativePath);

        // assert
        Assert.Equal($"{ExpectedRoot}/IO/Embedded/Resources/root.txt", actual);
    }

    [Theory]
    [InlineData(".")]
    [InlineData("./")]
    [InlineData("IO/..")]
    public void GetFullPath_WithPathResolvingToRoot_ReturnsRoot(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath(relativePath);

        // assert
        Assert.Equal(ExpectedRoot, actual);
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
    public void GetFullPath_WithEmptyOrWhiteSpacePath_ReturnsRoot(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath(relativePath);

        // assert
        Assert.Equal(sut.Root, actual);
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

        // act & assert
        var ex = Record.Exception(() => sut.GetFullPath("/IO/Embedded/Resources/root.txt"));

        Assert.IsType<ArgumentException>(ex);
    }

    [Theory]
    [InlineData("..")]
    [InlineData("../root.txt")]
    [InlineData("IO/../../root.txt")]
    [InlineData(@"IO\..\..\root.txt")]
    public void GetFullPath_WithPathNavigatingAboveRoot_ThrowsRelativePathException(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<RelativePathException>(
            () => sut.GetFullPath(relativePath),
            ex => ex.Message.Contains("escapes root") ? null : ex.Message
        );
    }

    // --- GetFile ---

    [Fact]
    public void GetFile_WithRelativePath_ReturnsEmbeddedFile() {
        // arrange
        var sut = CreateSut();

        var actual = sut.GetFile(
            SysPath.Combine("IO", "Embedded", "Resources", "root.txt")
        );

        // assert
        Assert.Multiple(
            () => Assert.IsType<File>(actual),
            () => Assert.Equal("root.txt", actual.Name),
            () => Assert.Equal($"{ExpectedRoot}/IO/Embedded/Resources/root.txt", actual.Path),
            () => Assert.True(actual.Exists)
        );
    }

    [Fact]
    public void GetFile_WithPathNavigatingAboveRoot_ThrowsRelativePathException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<RelativePathException>(
            () => sut.GetFile(SysPath.Combine("..", "root.txt")),
            ex => ex.Message.Contains("escapes root") ? null : ex.Message
        );
    }

    [Fact]
    public void GetFile_WithNullPath_ThrowsArgumentNullException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetFile(null!));
    }

    // --- GetDirectory ---

    [Fact]
    public void GetDirectory_WithPath_ReturnsEmbeddedDirectory() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetDirectory(
            SysPath.Combine("IO", "Embedded", "Resources", "folder-with-dash")
        );

        // assert
        Assert.Multiple(
            () => Assert.IsType<Directory>(actual),
            () => Assert.Equal("folder-with-dash", actual.Name),
            () => Assert.Equal($"{ExpectedRoot}/IO/Embedded/Resources/folder-with-dash", actual.Path),
            () => Assert.True(actual.Exists)
        );
    }

    [Fact]
    public void GetDirectory_WithDot_ReturnsRootDirectory() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetDirectory(".");

        // assert
        Assert.Multiple(
            () => Assert.Empty(actual.Name),
            () => Assert.Equal(ExpectedRoot, actual.Path),
            () => Assert.True(actual.Exists)
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
