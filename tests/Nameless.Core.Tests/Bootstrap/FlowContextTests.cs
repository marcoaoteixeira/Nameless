using Nameless.Bootstrap;

namespace Nameless;

public class FlowContextTests {
    // ─── Indexer set and get ─────────────────────────────────────────────────

    [Fact]
    public void Indexer_SetAndGet_StoresAndRetrievesValue() {
        // arrange
        var context = new FlowContext();

        // act
        context["key"] = "value";

        // assert
        Assert.Equal("value", context["key"]);
    }

    [Fact]
    public void Indexer_GetOnMissingKey_ReturnsNull() {
        // arrange
        var context = new FlowContext();

        // act
        var result = context["missing"];

        // assert
        Assert.Null(result);
    }

    // ─── Add(KeyValuePair) ───────────────────────────────────────────────────

    [Fact]
    public void Add_KeyValuePair_StoresValue() {
        // arrange
        var context = new FlowContext();

        // act
        context.Add(new KeyValuePair<string, object?>("foo", 42));

        // assert
        Assert.Equal(42, context["foo"]);
    }

    // ─── Keys ────────────────────────────────────────────────────────────────

    [Fact]
    public void Keys_ReturnsStoredKeys() {
        // arrange
        var context = new FlowContext();
        context["a"] = 1;
        context["b"] = 2;

        // act
        var keys = context.Keys.ToList();

        // assert
        Assert.Multiple(() => {
            Assert.Contains("a", keys);
            Assert.Contains("b", keys);
        });
    }

    // ─── Enumeration ─────────────────────────────────────────────────────────

    [Fact]
    public void Enumeration_IteratesAllKeyValuePairs() {
        // arrange
        var context = new FlowContext();
        context["x"] = 10;
        context["y"] = 20;

        // act
        var pairs = context.ToList();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(2, pairs.Count);
            Assert.Contains(pairs, p => p.Key == "x" && (int?)p.Value == 10);
            Assert.Contains(pairs, p => p.Key == "y" && (int?)p.Value == 20);
        });
    }

    [Fact]
    public void NonGenericGetEnumerator_IteratesEntries() {
        // arrange
        var context = new FlowContext();
        context["a"] = 1;

        // act — cast to IEnumerable to exercise the explicit interface method
        var enumerable = (System.Collections.IEnumerable)context;
        var count = 0;
        foreach (var _ in enumerable) { count++; }

        // assert
        Assert.Equal(1, count);
    }
}
