namespace Nameless;

public class CharExtensionsTests {
    // ─── IsDigit ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData('0', true)]
    [InlineData('9', true)]
    [InlineData('a', false)]
    [InlineData(' ', false)]
    public void IsDigit_ReturnsExpected(char c, bool expected) {
        Assert.Equal(expected, c.IsDigit());
    }

    // ─── IsLetter ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData('a', true)]
    [InlineData('Z', true)]
    [InlineData('1', false)]
    [InlineData(' ', false)]
    public void IsLetter_ReturnsExpected(char c, bool expected) {
        Assert.Equal(expected, c.IsLetter());
    }

    // ─── IsWhiteSpace ────────────────────────────────────────────────────────

    [Theory]
    [InlineData(' ', true)]
    [InlineData('\t', true)]
    [InlineData('a', false)]
    [InlineData('1', false)]
    public void IsWhiteSpace_ReturnsExpected(char c, bool expected) {
        Assert.Equal(expected, c.IsWhiteSpace());
    }

    // ─── IsUpper ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData('A', true)]
    [InlineData('Z', true)]
    [InlineData('a', false)]
    [InlineData('1', false)]
    public void IsUpper_ReturnsExpected(char c, bool expected) {
        Assert.Equal(expected, c.IsUpper());
    }

    // ─── IsLower ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData('a', true)]
    [InlineData('z', true)]
    [InlineData('A', false)]
    [InlineData('1', false)]
    public void IsLower_ReturnsExpected(char c, bool expected) {
        Assert.Equal(expected, c.IsLower());
    }
}
