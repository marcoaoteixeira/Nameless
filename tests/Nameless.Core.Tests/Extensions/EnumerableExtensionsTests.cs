using System.Collections;

namespace Nameless;

public class EnumerableExtensionsTests {
    // ─── Each(Action<T>) ─────────────────────────────────────────────────────

    [Fact]
    public void Each_Generic_CallsActionForEachItem() {
        // arrange
        var items = new List<int> { 1, 2, 3 };
        var collected = new List<int>();

        // act
        items.Each(collected.Add);

        // assert
        Assert.Equal([1, 2, 3], collected);
    }

    // ─── Each(Action<T, int>) ────────────────────────────────────────────────

    [Fact]
    public void Each_GenericWithIndex_PassesCorrectIndex() {
        // arrange
        var items = new List<string> { "a", "b", "c" };
        var indexed = new List<(string, int)>();

        // act
        items.Each((item, idx) => indexed.Add((item, idx)));

        // assert
        Assert.Multiple(() => {
            Assert.Equal(("a", 0), indexed[0]);
            Assert.Equal(("b", 1), indexed[1]);
            Assert.Equal(("c", 2), indexed[2]);
        });
    }

    // ─── Non-generic Each(Action<object?>) ───────────────────────────────────

    [Fact]
    public void Each_NonGeneric_IteratesAllItems() {
        // arrange
        IEnumerable items = new ArrayList { 1, "two", 3.0 };
        var collected = new List<object?>();

        // act
        items.Each(collected.Add);

        // assert
        Assert.Equal(3, collected.Count);
    }

    // ─── Non-generic Each(Action<object?, int>) ──────────────────────────────

    [Fact]
    public void Each_NonGenericWithIndex_PassesCorrectIndex() {
        // arrange
        IEnumerable items = new ArrayList { "x", "y" };
        var indices = new List<int>();

        // act
        items.Each((_, idx) => indices.Add(idx));

        // assert
        Assert.Equal([0, 1], indices);
    }

    // ─── IsNullOrEmpty ───────────────────────────────────────────────────────

    [Fact]
    public void IsNullOrEmpty_WithNull_ReturnsTrue() {
        // act & assert
        Assert.True(EnumerableExtensions.IsNullOrEmpty(null));
    }

    [Fact]
    public void IsNullOrEmpty_WithEmptyICollection_ReturnsTrue() {
        // arrange
        IEnumerable empty = new List<int>();

        // act & assert
        Assert.True(EnumerableExtensions.IsNullOrEmpty(empty));
    }

    [Fact]
    public void IsNullOrEmpty_WithNonEmptyCollection_ReturnsFalse() {
        // arrange
        IEnumerable nonEmpty = new List<int> { 1, 2 };

        // act & assert
        Assert.False(EnumerableExtensions.IsNullOrEmpty(nonEmpty));
    }

    [Fact]
    public void IsNullOrEmpty_WithEmptyEnumerable_ReturnsTrue() {
        // arrange
        static IEnumerable YieldNothing() { yield break; }

        // act & assert
        Assert.True(EnumerableExtensions.IsNullOrEmpty(YieldNothing()));
    }

    // ─── DistinctBy ──────────────────────────────────────────────────────────

    [Fact]
    public void DistinctBy_KeySelector_ReturnsOnlyDistinctItems() {
        // arrange
        var items = new List<string> { "apple", "ant", "banana", "avocado" };

        // act
        var result = items.DistinctBy(s => s[0]).ToList();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(2, result.Count);
            Assert.Contains("apple", result);
            Assert.Contains("banana", result);
        });
    }

    // ─── WithIndex ───────────────────────────────────────────────────────────

    [Fact]
    public void WithIndex_PairsEachItemWithZeroBasedIndex() {
        // arrange
        var items = new List<string> { "a", "b", "c" };

        // act
        var result = items.WithIndex().ToList();

        // assert
        Assert.Multiple(() => {
            Assert.Equal((0, "a"), result[0]);
            Assert.Equal((1, "b"), result[1]);
            Assert.Equal((2, "c"), result[2]);
        });
    }
}
