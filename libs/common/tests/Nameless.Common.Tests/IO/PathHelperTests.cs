namespace Nameless.IO;

[UnitTest]
public class PathHelperTests {
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
}
