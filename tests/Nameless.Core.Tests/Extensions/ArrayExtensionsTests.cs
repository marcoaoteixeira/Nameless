namespace Nameless;

public class ArrayExtensionsTests {
    // ─── TryGetElementAt ────────────────────────────────────────────────────

    [Fact]
    public void TryGetElementAt_WithValidIndex_ReturnsTrueAndValue() {
        // arrange
        var array = new[] { "a", "b", "c" };

        // act
        var found = array.TryGetElementAt(index: 1, out var value);

        // assert
        Assert.Multiple(() => {
            Assert.True(found);
            Assert.Equal("b", value);
        });
    }

    [Fact]
    public void TryGetElementAt_WithIndexZero_ReturnsFalse() {
        // arrange
        var array = new[] { "a", "b", "c" };

        // act
        var found = array.TryGetElementAt(index: 0, out var value);

        // assert
        Assert.Multiple(() => {
            Assert.False(found);
            Assert.Null(value);
        });
    }

    [Fact]
    public void TryGetElementAt_WithNegativeIndex_ReturnsFalseAndDefault() {
        // arrange
        var array = new[] { "a", "b", "c" };

        // act
        var found = array.TryGetElementAt(index: -1, out var value);

        // assert
        Assert.Multiple(() => {
            Assert.False(found);
            Assert.Null(value);
        });
    }

    [Fact]
    public void TryGetElementAt_WithIndexEqualToLength_ReturnsFalse() {
        // arrange
        var array = new[] { "a", "b", "c" };

        // act
        var found = array.TryGetElementAt(index: 3, out var value);

        // assert
        Assert.False(found);
    }

    // ─── IsInRange ──────────────────────────────────────────────────────────

    [Fact]
    public void IsInRange_WithIndexInMiddle_ReturnsTrue() {
        // arrange
        var array = new[] { 1, 2, 3, 4, 5 };

        // act & assert
        Assert.True(array.IsInRange(2));
    }

    [Fact]
    public void IsInRange_WithIndexZero_ReturnsFalse() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act & assert
        Assert.False(array.IsInRange(0));
    }

    [Fact]
    public void IsInRange_WithNegativeIndex_ReturnsFalse() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act & assert
        Assert.False(array.IsInRange(-1));
    }

    [Fact]
    public void IsInRange_WithIndexEqualToLength_ReturnsFalse() {
        // arrange
        var array = new[] { 1, 2, 3 };

        // act & assert
        Assert.False(array.IsInRange(3));
    }
}
