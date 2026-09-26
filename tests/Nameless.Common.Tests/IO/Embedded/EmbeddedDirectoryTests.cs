using System.Reflection;

namespace Nameless.IO.Embedded;

[UnitTest]
public class EmbeddedDirectoryTests {
    private const string Resources = "IO/Embedded/Resources";

    private static readonly Assembly TestAssembly = typeof(EmbeddedDirectoryTests).Assembly;

    private readonly EmbeddedFileProvider _provider = new(TestAssembly);

    private string[] GetFilePaths(IDirectory directory, string glob) {
        return directory.GetFiles(glob)
                        .Select(file => file.Path[_provider.Root.Length..])
                        .Order(StringComparer.Ordinal)
                        .ToArray();
    }

    // --- Properties ---

    [Fact]
    public void Name_ReturnsDirectoryName() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/folder-with-dash/deeper");

        // act
        var actual = sut.Name;

        // assert
        Assert.Equal("deeper", actual);
    }

    [Fact]
    public void Path_ReturnsRootCombinedWithRelativePath() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/folder-with-dash");

        // act
        var actual = sut.Path;

        // assert
        Assert.Equal($"{_provider.Root}{Resources}/folder-with-dash", actual);
    }

    [Fact]
    public void Exists_WhenDirectoryIsEmbedded_ReturnsTrue() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/folder-with-dash");

        // act
        var actual = sut.Exists;

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void Exists_WhenDirectoryIsNotEmbedded_ReturnsFalse() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/missing");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void Exists_WhenPathIsFile_ReturnsFalse() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/root.txt");

        // act
        var actual = sut.Exists;

        // assert
        Assert.False(actual);
    }

    // --- Unsupported operations ---

    [Fact]
    public void Create_ThrowsInvalidOperationException() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/new");

        // act & assert
        Assert.Throws<InvalidOperationException>(sut.Create);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Delete_ThrowsInvalidOperationException(bool recursive) {
        // arrange
        var sut = _provider.GetDirectory(Resources);

        // act & assert
        Assert.Throws<InvalidOperationException>(() => sut.Delete(recursive));
    }

    // --- GetFiles ---

    [Fact]
    public void GetFiles_WithNullGlob_ThrowsArgumentNullException() {
        // arrange
        var sut = _provider.GetDirectory(Resources);

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetFiles(null!).ToList());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GetFiles_WithEmptyOrWhiteSpaceGlob_ThrowsArgumentException(string glob) {
        // arrange
        var sut = _provider.GetDirectory(Resources);

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetFiles(glob).ToList());
    }

    [Fact]
    public void GetFiles_WithTopLevelGlob_ReturnsOnlyMatchingFilesInDirectory() {
        // arrange
        var sut = _provider.GetDirectory(Resources);

        // act
        var actual = GetFilePaths(sut, "*.txt");

        // assert
        Assert.Equal([$"{Resources}/root.txt"], actual);
    }

    [Fact]
    public void GetFiles_WithRecursiveGlob_ReturnsMatchingFilesInSubdirectories() {
        // arrange
        var sut = _provider.GetDirectory(Resources);

        // act
        var actual = GetFilePaths(sut, "**/*.txt");

        // assert
        Assert.Equal([
            $"{Resources}/folder-with-dash/deeper/deep.txt",
            $"{Resources}/folder-with-dash/nested.txt",
            $"{Resources}/root.txt"
        ], actual);
    }

    [Fact]
    public void GetFiles_FromSubdirectory_ReturnsFilesRelativeToProviderRoot() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/folder-with-dash");

        // act
        var actual = GetFilePaths(sut, "deeper/*");

        // assert
        Assert.Equal([
            $"{Resources}/folder-with-dash/deeper/deep.log",
            $"{Resources}/folder-with-dash/deeper/deep.txt"
        ], actual);
    }

    [Fact]
    public void GetFiles_ReturnsExistingFilesWithName() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/folder-with-dash");

        // act
        var actual = Assert.Single(sut.GetFiles("*"));

        // assert
        Assert.Multiple(
            () => Assert.True(actual.Exists),
            () => Assert.Equal("nested.txt", actual.Name)
        );
    }

    [Fact]
    public void GetFiles_IsCaseInsensitive() {
        // arrange
        var sut = _provider.GetDirectory(Resources);

        // act
        var actual = GetFilePaths(sut, "**/*.TXT");

        // assert
        Assert.Equal([
            $"{Resources}/folder-with-dash/deeper/deep.txt",
            $"{Resources}/folder-with-dash/nested.txt",
            $"{Resources}/root.txt"
        ], actual);
    }

    [Fact]
    public void GetFiles_WhenDirectoryIsNotEmbedded_ReturnsEmpty() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/missing");

        // act
        var actual = sut.GetFiles("**/*");

        // assert
        Assert.Empty(actual);
    }

    [Fact]
    public void GetFiles_WhenPathIsFile_ReturnsEmpty() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/root.txt");

        // act
        var actual = sut.GetFiles("**/*");

        // assert
        Assert.Empty(actual);
    }

    [Fact]
    public void GetFiles_FromRootDirectory_ReturnsAllEmbeddedFixtures() {
        // arrange
        var sut = _provider.GetDirectory(".");

        // act
        var actual = GetFilePaths(sut, $"{Resources}/**/*");

        // assert
        Assert.Equal([
            $"{Resources}/data.json",
            $"{Resources}/folder-with-dash/deeper/deep.log",
            $"{Resources}/folder-with-dash/deeper/deep.txt",
            $"{Resources}/folder-with-dash/nested.txt",
            $"{Resources}/root.txt"
        ], actual);
    }

    // --- Extensions ---

    [Fact]
    public void IsEmpty_WhenDirectoryHasTopLevelFiles_ReturnsFalse() {
        // arrange
        var sut = _provider.GetDirectory(Resources);

        // act
        var actual = sut.IsEmpty;

        // assert
        Assert.False(actual);
    }

    [Fact]
    public void IsEmpty_WhenDirectoryIsNotEmbedded_ReturnsTrue() {
        // arrange
        var sut = _provider.GetDirectory($"{Resources}/missing");

        // act
        var actual = sut.IsEmpty;

        // assert
        Assert.True(actual);
    }
}
