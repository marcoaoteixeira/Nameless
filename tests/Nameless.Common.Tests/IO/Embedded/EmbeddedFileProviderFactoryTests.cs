using System.Reflection;

namespace Nameless.IO.Embedded;

[UnitTest]
public class EmbeddedFileProviderFactoryTests {
    private static readonly Assembly TestAssembly = typeof(EmbeddedFileProviderFactoryTests).Assembly;

    [Fact]
    public void GetOrCreate_ReturnsProviderForAssembly() {
        // arrange
        var sut = new EmbeddedFileProviderFactory();

        // act
        var actual = sut.GetOrCreate(TestAssembly);

        // assert
        Assert.Equal($"embedded://{TestAssembly.GetName().Name}/", actual.Root);
    }

    [Fact]
    public void GetOrCreate_ForSameAssembly_ReturnsCachedProvider() {
        // arrange
        var sut = new EmbeddedFileProviderFactory();

        // act
        var first = sut.GetOrCreate(TestAssembly);
        var second = sut.GetOrCreate(TestAssembly);

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void GetOrCreate_OnDifferentFactoryInstances_DoesNotShareCache() {
        // act
        var first = new EmbeddedFileProviderFactory().GetOrCreate(TestAssembly);
        var second = new EmbeddedFileProviderFactory().GetOrCreate(TestAssembly);

        // assert
        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetOrCreate_WithNullAssembly_ThrowsArgumentNullException() {
        // arrange
        var sut = new EmbeddedFileProviderFactory();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetOrCreate(null!));
    }

    [Fact]
    public void GetOrCreate_WithAssemblyWithoutManifest_ThrowsAndDoesNotCache() {
        // arrange
        var sut = new EmbeddedFileProviderFactory();

        // act
        var first = Record.Exception(() => sut.GetOrCreate(typeof(object).Assembly));
        var second = Record.Exception(() => sut.GetOrCreate(typeof(object).Assembly));

        // assert
        Assert.Multiple(
            () => Assert.IsType<InvalidOperationException>(first),
            () => Assert.IsType<InvalidOperationException>(second)
        );
    }
}
