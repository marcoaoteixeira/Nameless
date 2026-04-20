using Nameless.ProducerConsumer;

namespace Nameless;

public class ContextTests {
    // --- ConsumerContext ---

    [Fact]
    public void ConsumerContext_DefaultConstructor_IsEmpty() {
        // act
        var ctx = new ConsumerContext();

        // assert
        Assert.Empty(ctx);
    }

    [Fact]
    public void ConsumerContext_WithDictionary_ContainsEntries() {
        // arrange
        var dict = new Dictionary<string, object?> { ["key"] = "value" };

        // act
        var ctx = new ConsumerContext(dict);

        // assert
        Assert.Equal("value", ctx["key"]);
    }

    // --- ProducerContext ---

    [Fact]
    public void ProducerContext_DefaultConstructor_IsEmpty() {
        // act
        var ctx = new ProducerContext();

        // assert
        Assert.Empty(ctx);
    }

    [Fact]
    public void ProducerContext_WithDictionary_ContainsEntries() {
        // arrange
        var dict = new Dictionary<string, object?> { ["topic"] = "orders" };

        // act
        var ctx = new ProducerContext(dict);

        // assert
        Assert.Equal("orders", ctx["topic"]);
    }

    // --- Context indexer and Add ---

    [Fact]
    public void Context_Indexer_SetAndGet_RoundTrips() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx["myKey"] = 42;

        // assert
        Assert.Equal(42, ctx["myKey"]);
    }

    [Fact]
    public void Context_Indexer_IsCaseInsensitive() {
        // arrange
        var ctx = new ConsumerContext();
        ctx["MyKey"] = "hello";

        // act & assert
        Assert.Equal("hello", ctx["mykey"]);
        Assert.Equal("hello", ctx["MYKEY"]);
    }

    [Fact]
    public void Context_Add_AddsKeyValuePair() {
        // arrange
        var ctx = new ConsumerContext();

        // act
        ctx.Add(new KeyValuePair<string, object?>("name", "world"));

        // assert
        Assert.Equal("world", ctx["name"]);
    }

    [Fact]
    public void Context_Indexer_WithNullOrWhitespaceKey_Throws() {
        // arrange
        var ctx = new ConsumerContext();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => ctx[null!] = "value");
    }

    [Fact]
    public void Context_GetEnumerator_IteratesAllEntries() {
        // arrange
        var dict = new Dictionary<string, object?> {
            ["a"] = 1,
            ["b"] = 2
        };
        var ctx = new ConsumerContext(dict);

        // act
        var entries = ctx.ToList();

        // assert
        Assert.Equal(2, entries.Count);
    }

    [Fact]
    public void Context_NonGenericGetEnumerator_IteratesEntries() {
        // cast to IEnumerable to exercise the explicit non-generic interface method
        var ctx = new ConsumerContext(new Dictionary<string, object?> { ["k"] = "v" });

        var enumerable = (System.Collections.IEnumerable)ctx;
        var count = 0;
        foreach (var _ in enumerable) { count++; }

        Assert.Equal(1, count);
    }
}
