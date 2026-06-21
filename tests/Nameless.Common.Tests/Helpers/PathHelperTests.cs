namespace Nameless.Helpers;

public class PathHelperTests {
    // ─── Normalize ───────────────────────────────────────────────────────────

    [Fact]
    public void Normalize_ForwardSlashes_ReplacedWithDirectorySeparatorChar() {
        // arrange
        const string input = "a/b/c";

        // act
        var result = PathHelper.Normalize(input);

        // assert
        Assert.Equal($"a{Path.DirectorySeparatorChar}b{Path.DirectorySeparatorChar}c", result);
    }

    [Fact]
    public void Normalize_MixedSlashes_ReplacedWithDirectorySeparatorChar() {
        // arrange
        var input = $"a{Path.AltDirectorySeparatorChar}b{Path.DirectorySeparatorChar}c";

        // act
        var result = PathHelper.Normalize(input);

        // assert
        Assert.Equal($"a{Path.DirectorySeparatorChar}b{Path.DirectorySeparatorChar}c", result);
    }

    // ─── Sanitize ────────────────────────────────────────────────────────────

    [Fact]
    public void Sanitize_ValidPath_ReturnsSameString() {
        // arrange
        const string valid = "valid_path";

        // act
        var result = PathHelper.Sanitize(valid);

        // assert
        Assert.Equal(valid, result);
    }

    [Fact]
    public void Sanitize_WithNull_ReturnsNull() {
        // act
        var result = PathHelper.Sanitize(null!);

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void Sanitize_WithWhitespaceOnly_ReturnsWhitespace() {
        // arrange
        const string whitespace = "   ";

        // act
        var result = PathHelper.Sanitize(whitespace);

        // assert
        Assert.Equal(whitespace, result);
    }

    [Fact]
    public void Sanitize_PathWithInvalidChars_ReplacesWithUnderscore() {
        // arrange
        var invalidChar = Path.GetInvalidPathChars().First(c => c != '_');
        var input = $"valid{invalidChar}path";

        // act
        var result = PathHelper.Sanitize(input);

        // assert
        Assert.Equal("valid_path", result);
    }

    [Fact]
    public void Sanitize_ReplacementIsInvalidChar_ThrowsArgumentException() {
        // arrange
        var invalidReplacement = Path.GetInvalidPathChars()[0];

        // act & assert
        Assert.Throws<ArgumentException>(
            () => PathHelper.Sanitize("somepath", replacement: invalidReplacement)
        );
    }
}
