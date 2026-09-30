namespace Nameless.IO;

[UnitTest]
public class PathHelperTests {
    // ─── Normalize ───────────────────────────────────────────────────────────

    [Fact]
    public void Normalize_ForwardSlashes_ReplacedWithDirectorySeparatorChar() {
        // arrange
        const string Input = "a/b/c";

        // act
        var result = PathHelper.Normalize(Input);

        // assert
        Assert.Equal($"a{SysPath.DirectorySeparatorChar}b{SysPath.DirectorySeparatorChar}c", result);
    }

    [Fact]
    public void Normalize_MixedSlashes_ReplacedWithDirectorySeparatorChar() {
        // arrange
        var input = $"a{SysPath.AltDirectorySeparatorChar}b{SysPath.DirectorySeparatorChar}c";

        // act
        var result = PathHelper.Normalize(input);

        // assert
        Assert.Equal($"a{SysPath.DirectorySeparatorChar}b{SysPath.DirectorySeparatorChar}c", result);
    }

    // ─── Sanitize ────────────────────────────────────────────────────────────

    [Fact]
    public void Sanitize_ValidPath_ReturnsSameString() {
        // arrange
        const string Valid = "valid_path";

        // act
        var result = PathHelper.Sanitize(Valid);

        // assert
        Assert.Equal(Valid, result);
    }

    [Fact]
    public void Sanitize_WithNull_ThrowsArgumentNullException() {
        // act && assert
        Assert.Throws<ArgumentNullException>(() => PathHelper.Sanitize(null!));
    }

    [Fact]
    public void Sanitize_WithWhitespaceOnly_ReturnsWhitespace() {
        // arrange
        const string Whitespace = "   ";

        // act
        var result = PathHelper.Sanitize(Whitespace);

        // assert
        Assert.Equal(Whitespace, result);
    }

    [Fact]
    public void Sanitize_PathWithInvalidChars_ReplacesWithUnderscore() {
        // arrange
        var invalidChar = SysPath.GetInvalidPathChars().First(c => c != '_');
        var input = $"valid{invalidChar}path";

        // act
        var result = PathHelper.Sanitize(input);

        // assert
        Assert.Equal("valid_path", result);
    }

    [Fact]
    public void Sanitize_ReplacementIsInvalidChar_ThrowsArgumentException() {
        // arrange
        var invalidReplacement = SysPath.GetInvalidPathChars()[0];

        // act & assert
        Assert.Throws<ArgumentException>(
            () => PathHelper.Sanitize("somepath", replacement: invalidReplacement)
        );
    }

    // ─── RemoveTrailingSlash ─────────────────────────────────────────────────

    [Theory]
    [InlineData("a/b/", "a/b")]
    [InlineData("a/b//", "a/b")]
    [InlineData("a/b", "a/b")]
    public void RemoveTrailingSlash_WithTrailingSeparators_RemovesThem(string path, string expected) {
        // act
        var result = PathHelper.RemoveTrailingSlash(path);

        // assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveTrailingSlash_WithLeadingSeparator_KeepsIt() {
        // act
        var result = PathHelper.RemoveTrailingSlash("/var/data/");

        // assert
        Assert.Equal("/var/data", result);
    }

    [Theory]
    [InlineData(@"a\b\\", @"a\b")]
    [InlineData(@"\\server\share\", @"\\server\share")]
    public void RemoveTrailingSlash_WithBackslashes_OnWindows_RemovesOnlyTrailingSeparators(string path, string expected) {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Backslash is a separator only on Windows.");

        // act
        var result = PathHelper.RemoveTrailingSlash(path);

        // assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveTrailingSlash_WithVolumeRootOnly_ReturnsVolumeRoot() {
        // arrange
        var volumeRoot = SysPath.GetPathRoot(SysPath.GetTempPath())!;

        // act
        var result = PathHelper.RemoveTrailingSlash(volumeRoot);

        // assert
        Assert.Equal(volumeRoot, result);
    }

    [Fact]
    public void RemoveTrailingSlash_WithNull_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => PathHelper.RemoveTrailingSlash(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RemoveTrailingSlash_WithEmptyOrWhiteSpace_ThrowsArgumentException(string path) {
        // act & assert
        Assert.Throws<ArgumentException>(() => PathHelper.RemoveTrailingSlash(path));
    }

    // ─── ResolveRelativePath ─────────────────────────────────────────────────

    [Theory]
    [InlineData("..")]
    [InlineData("../file.txt")]
    [InlineData("sub/../../file.txt")]
    public void ResolveRelativePath_WithForwardSlashNavigatingAboveRoot_ThrowsRelativePathException(string relativePath) {
        // act & assert
        Assert.Throws<RelativePathException>(() => PathHelper.ResolveRelativePath(relativePath));
    }

    [Theory]
    [InlineData(@"..\file.txt")]
    [InlineData(@"sub\..\../file.txt")]
    public void ResolveRelativePath_WithMixedSeparatorsNavigatingAboveRoot_OnWindows_ThrowsRelativePathException(string relativePath) {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Backslash is a separator only on Windows.");

        // act & assert
        Assert.Throws<RelativePathException>(() => PathHelper.ResolveRelativePath(relativePath));
    }

    [Fact]
    public void ResolveRelativePath_WithForwardSlashes_ReturnsPathJoinedByDirectorySeparator() {
        // arrange
        var sep = SysPath.DirectorySeparatorChar;

        // act
        var result = PathHelper.ResolveRelativePath("a/b/../c/./d");

        // assert
        Assert.Equal($"a{sep}c{sep}d", result);
    }

    [Fact]
    public void ResolveRelativePath_WithMixedSeparators_OnWindows_ReturnsPathJoinedByDirectorySeparator() {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Backslash is a separator only on Windows.");

        // act
        var result = PathHelper.ResolveRelativePath(@"a/b\..\c/./d");

        // assert
        Assert.Equal(@"a\c\d", result);
    }
}
