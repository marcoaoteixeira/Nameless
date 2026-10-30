using Microsoft.Extensions.Logging;

namespace Nameless.Testing.Tools.Logging;

[UnitTest]
public class InMemoryLoggerFactoryTests {
    // ─── CreateLogger ───────────────────────────────────────────────────────

    [Fact]
    public void CreateLogger_ReturnsInMemoryLogger() {
        var sut = new InMemoryLoggerFactory();

        var result = sut.CreateLogger("category");

        Assert.IsType<InMemoryLogger>(result);
    }

    [Fact]
    public void CreateLogger_CalledTwiceWithSameCategory_ReturnsSameInstance() {
        var sut = new InMemoryLoggerFactory();

        var first = sut.CreateLogger("category");
        var second = sut.CreateLogger("category");

        Assert.Same(first, second);
    }

    [Fact]
    public void CreateLogger_CalledWithDifferentCategories_ReturnsDifferentInstances() {
        var sut = new InMemoryLoggerFactory();

        var first = sut.CreateLogger("category-a");
        var second = sut.CreateLogger("category-b");

        Assert.NotSame(first, second);
    }

    // ─── GetLogger ──────────────────────────────────────────────────────────

    [Fact]
    public void GetLogger_ReturnsSameInstanceAsCreateLogger() {
        var sut = new InMemoryLoggerFactory();

        var createdLogger = sut.CreateLogger("category");
        var fetchedLogger = sut.GetLogger("category");

        Assert.Same(createdLogger, fetchedLogger);
    }

    [Fact]
    public void GetLoggerOfT_ReturnsTypedInMemoryLogger() {
        var sut = new InMemoryLoggerFactory();

        var result = sut.GetLogger<InMemoryLoggerFactoryTests>();

        Assert.IsType<InMemoryLogger<InMemoryLoggerFactoryTests>>(result);
    }

    [Fact]
    public void GetLoggerOfT_CalledTwice_ReturnsSameInstance() {
        var sut = new InMemoryLoggerFactory();

        var first = sut.GetLogger<InMemoryLoggerFactoryTests>();
        var second = sut.GetLogger<InMemoryLoggerFactoryTests>();

        Assert.Same(first, second);
    }

    [Fact]
    public void GetLoggerOfT_SharesInstanceWithCreateLoggerUsingSameCategoryName() {
        var sut = new InMemoryLoggerFactory();
        var categoryName = typeof(InMemoryLoggerFactoryTests).FullName!;

        var typed = sut.GetLogger<InMemoryLoggerFactoryTests>();
        var byName = sut.CreateLogger(categoryName);

        Assert.Same(typed, byName);
    }

    // ─── Loggers ────────────────────────────────────────────────────────────

    [Fact]
    public void Loggers_ReflectsEveryCreatedLogger() {
        var sut = new InMemoryLoggerFactory();

        sut.CreateLogger("category-a");
        sut.CreateLogger("category-b");

        Assert.Multiple(
            () => Assert.Equal(2, sut.Loggers.Count),
            () => Assert.True(sut.Loggers.ContainsKey("category-a")),
            () => Assert.True(sut.Loggers.ContainsKey("category-b"))
        );
    }

    // ─── AddProvider ────────────────────────────────────────────────────────

    [Fact]
    public void AddProvider_DoesNotThrow() {
        var sut = new InMemoryLoggerFactory();
        using var provider = new NoOpLoggerProvider();

        sut.AddProvider(provider);
    }

    // ─── Dispose ────────────────────────────────────────────────────────────

    [Fact]
    public void Dispose_ClearsLoggers() {
        var sut = new InMemoryLoggerFactory();
        sut.CreateLogger("category");

        sut.Dispose();

        Assert.Empty(sut.Loggers);
    }

    // ─── test doubles ───────────────────────────────────────────────────────

    private sealed class NoOpLoggerProvider : ILoggerProvider {
        public ILogger CreateLogger(string categoryName) => throw new NotSupportedException();

        public void Dispose() { }
    }
}
