namespace Nameless;

public class NothingTests {
    // ─── Equality operators ──────────────────────────────────────────────────

    [Fact]
    public void EqualEqual_TwoNothingValues_ReturnsTrue() {
        // act & assert
        Assert.True(Nothing.Value == Nothing.Value);
    }

    [Fact]
    public void NotEqual_TwoNothingValues_ReturnsFalse() {
        // act & assert
        Assert.False(Nothing.Value != Nothing.Value);
    }

    // ─── Comparison operators ────────────────────────────────────────────────

    [Fact]
    public void LessThan_TwoNothingValues_ReturnsFalse() {
        // act & assert
        Assert.False(Nothing.Value < Nothing.Value);
    }

    [Fact]
    public void GreaterThan_TwoNothingValues_ReturnsFalse() {
        // act & assert
        Assert.False(Nothing.Value > Nothing.Value);
    }

    [Fact]
    public void LessThanOrEqual_TwoNothingValues_ReturnsTrue() {
        // act & assert
        Assert.True(Nothing.Value <= Nothing.Value);
    }

    [Fact]
    public void GreaterThanOrEqual_TwoNothingValues_ReturnsTrue() {
        // act & assert
        Assert.True(Nothing.Value >= Nothing.Value);
    }

    // ─── CompareTo ───────────────────────────────────────────────────────────

    [Fact]
    public void CompareTo_AnotherNothing_ReturnsZero() {
        // act
        var result = Nothing.Value.CompareTo(Nothing.Value);

        // assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void IComparable_CompareTo_Null_ReturnsOne() {
        // arrange
        IComparable comparable = Nothing.Value;

        // act
        var result = comparable.CompareTo(null);

        // assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void IComparable_CompareTo_WrongType_ThrowsArgumentException() {
        // arrange
        IComparable comparable = Nothing.Value;

        // act & assert
        Assert.Throws<ArgumentException>(() => comparable.CompareTo("not nothing"));
    }

    // ─── Equals ──────────────────────────────────────────────────────────────

    [Fact]
    public void Equals_AnotherNothing_ReturnsTrue() {
        // act & assert
        Assert.True(Nothing.Value.Equals(Nothing.Value));
    }

    [Fact]
    public void Equals_ObjectNothing_ReturnsTrue() {
        // arrange
        object other = Nothing.Value;

        // act & assert
        Assert.True(Nothing.Value.Equals(other));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse() {
        // act & assert
        Assert.False(Nothing.Value.Equals(null));
    }

    // ─── GetHashCode ─────────────────────────────────────────────────────────

    [Fact]
    public void GetHashCode_ReturnsZero() {
        // act & assert
        Assert.Equal(0, Nothing.Value.GetHashCode());
    }

    // ─── ToString ────────────────────────────────────────────────────────────

    [Fact]
    public void ToString_ReturnsParentheses() {
        // act & assert
        Assert.Equal("()", Nothing.Value.ToString());
    }

    // ─── Then ────────────────────────────────────────────────────────────────

    [Fact]
    public void Then_ExecutesActionAndReturnsNothingValue() {
        // arrange
        var called = false;

        // act
        var result = Nothing.Then(() => called = true);

        // assert
        Assert.Multiple(() => {
            Assert.True(called);
            Assert.Equal(Nothing.Value, result);
        });
    }
}
