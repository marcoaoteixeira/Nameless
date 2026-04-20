namespace Nameless;

public class DateOnlyExtensionsTests {
    // ─── ToUnixTimeMilliseconds ──────────────────────────────────────────────

    [Fact]
    public void ToUnixTimeMilliseconds_WithUnixEpochDate_ReturnsZero() {
        // arrange
        var epoch = new DateOnly(1970, 1, 1);

        // act
        var ms = epoch.ToUnixTimeMilliseconds();

        // assert
        Assert.Equal(0L, ms);
    }

    [Fact]
    public void ToUnixTimeMilliseconds_WithKnownDate_ReturnsCorrectMs() {
        // arrange
        var date = new DateOnly(2000, 1, 1);
        var expected = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();

        // act
        var ms = date.ToUnixTimeMilliseconds();

        // assert
        Assert.Equal(expected, ms);
    }

    [Fact]
    public void ToUnixTimeMilliseconds_WithLaterDate_ReturnsLargerValue() {
        // arrange
        var earlier = new DateOnly(2020, 1, 1);
        var later = new DateOnly(2021, 1, 1);

        // act & assert
        Assert.True(later.ToUnixTimeMilliseconds() > earlier.ToUnixTimeMilliseconds());
    }
}
