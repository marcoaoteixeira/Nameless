namespace Nameless.Extensions;

public class IntegerExtensionsTests {
    // ─── Times(Action) ───────────────────────────────────────────────────────

    [Fact]
    public void Times_WithAction_ExecutesCorrectNumberOfTimes() {
        // arrange
        var count = 0;

        // act
        3.Times(() => count++);

        // assert
        Assert.Equal(3, count);
    }

    [Fact]
    public void Times_WithZero_ExecutesNever() {
        // arrange
        var count = 0;

        // act
        0.Times(() => count++);

        // assert
        Assert.Equal(0, count);
    }

    // ─── Times(Action<int>) ──────────────────────────────────────────────────

    [Fact]
    public void Times_WithIndexedAction_PassesCorrectIndices() {
        // arrange
        var indices = new List<int>();

        // act
        3.Times(indices.Add);

        // assert
        Assert.Equal([0, 1, 2], indices);
    }

    // ─── IsWithinRange ───────────────────────────────────────────────────────

    [Fact]
    public void IsWithinRange_WhenValueIsInsideRange_ReturnsTrue() {
        Assert.True(5.IsWithinRange(1, 10));
    }

    [Fact]
    public void IsWithinRange_WhenValueIsAtMinBoundary_ReturnsTrueByDefault() {
        Assert.True(1.IsWithinRange(1, 10));
    }

    [Fact]
    public void IsWithinRange_WhenValueIsAtMaxBoundary_ReturnsTrueByDefault() {
        Assert.True(10.IsWithinRange(1, 10));
    }

    [Fact]
    public void IsWithinRange_WhenValueBelowRange_ReturnsFalse() {
        Assert.False(0.IsWithinRange(1, 10));
    }

    [Fact]
    public void IsWithinRange_WhenValueAboveRange_ReturnsFalse() {
        Assert.False(11.IsWithinRange(1, 10));
    }

    [Fact]
    public void IsWithinRange_WithIncludeLimitFalse_ExcludesBoundaries() {
        Assert.Multiple(() => {
            Assert.False(1.IsWithinRange(1, 10, includeLimit: false));
            Assert.False(10.IsWithinRange(1, 10, includeLimit: false));
            Assert.True(5.IsWithinRange(1, 10, includeLimit: false));
        });
    }
}
