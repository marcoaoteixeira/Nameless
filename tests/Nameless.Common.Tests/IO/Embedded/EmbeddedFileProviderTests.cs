using System.Reflection;

namespace Nameless.IO.Embedded;

[UnitTest]
public class EmbeddedFileProviderTests {
    private static readonly Assembly TestAssembly = typeof(EmbeddedFileProviderTests).Assembly;
    private static readonly string ExpectedRoot = $"embedded://{TestAssembly.GetName().Name}/";

    private static EmbeddedFileProvider CreateSut() {
        return new EmbeddedFileProvider(TestAssembly);
    }

    // --- Constructor ---

    [Fact]
    public void Constructor_WithNullAssembly_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new EmbeddedFileProvider(null!));
    }

    [Fact]
    public void Constructor_WithAssemblyWithoutManifest_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new EmbeddedFileProvider(typeof(object).Assembly));
    }

    [Fact]
    public void Root_ReturnsEmbeddedSchemeWithAssemblyName() {
        // act
        var actual = CreateSut().Root;

        // assert
        Assert.Equal(ExpectedRoot, actual);
    }

    // --- GetFullPath ---

    [Fact]
    public void GetFullPath_WithRelativePath_ReturnsPathCombinedWithRoot() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFullPath("IO/Embedded/Resources/root.txt");

        // assert
        Assert.Equal($"{ExpectedRoot}IO/Embedded/Resources/root.txt", actual);
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
        Assert.Equal($"{ExpectedRoot}IO/Embedded/Resources/root.txt", actual);
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

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetFullPath("/IO/Embedded/Resources/root.txt"));
    }

    [Theory]
    [InlineData("..")]
    [InlineData("../root.txt")]
    [InlineData("IO/../../root.txt")]
    [InlineData(@"IO\..\..\root.txt")]
    public void GetFullPath_WithPathNavigatingAboveRoot_ThrowsUnauthorizedAccessException(string relativePath) {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<UnauthorizedAccessException>(() => sut.GetFullPath(relativePath));
    }

    // --- GetFile ---

    [Fact]
    public void GetFile_WithRelativePath_ReturnsEmbeddedFile() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetFile(@"IO\Embedded\Resources\root.txt");

        // assert
        Assert.Multiple(
            () => Assert.IsType<EmbeddedFile>(actual),
            () => Assert.Equal("root.txt", actual.Name),
            () => Assert.Equal($"{ExpectedRoot}IO/Embedded/Resources/root.txt", actual.Path),
            () => Assert.True(actual.Exists)
        );
    }

    [Fact]
    public void GetFile_WithPathNavigatingAboveRoot_ThrowsUnauthorizedAccessException() {
        // arrange
        var sut = CreateSut();

        // act & assert
        Assert.Throws<UnauthorizedAccessException>(() => sut.GetFile("../root.txt"));
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
    public void GetDirectory_WithRelativePath_ReturnsEmbeddedDirectory() {
        // arrange
        var sut = CreateSut();

        // act
        var actual = sut.GetDirectory("IO/Embedded/Resources/folder-with-dash");

        // assert
        Assert.Multiple(
            () => Assert.IsType<EmbeddedDirectory>(actual),
            () => Assert.Equal("folder-with-dash", actual.Name),
            () => Assert.Equal($"{ExpectedRoot}IO/Embedded/Resources/folder-with-dash", actual.Path),
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
            () => Assert.Equal(TestAssembly.GetName().Name, actual.Name),
            () => Assert.Equal(ExpectedRoot, actual.Path),
            () => Assert.True(actual.Exists)
        );
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
