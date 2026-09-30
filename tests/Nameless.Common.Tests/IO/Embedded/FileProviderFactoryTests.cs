using System.Reflection;

namespace Nameless.IO.Embedded;

[UnitTest]
public class FileProviderFactoryTests {
    private static readonly Assembly TestAssembly = typeof(FileProviderFactoryTests).Assembly;
    private static readonly string ExpectedRoot = $"embedded://{TestAssembly.GetName().Name}";

    [Fact]
    public void GetOrCreate_WithDefaultRoot_ReturnsProviderForAssembly() {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var actual = sut.GetOrCreate(TestAssembly);

        // assert
        Assert.Equal(ExpectedRoot, actual.Root);
    }

    [Fact]
    public void GetOrCreate_ForSameAssemblyAndRoot_ReturnsCachedProvider() {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var first = sut.GetOrCreate(TestAssembly, "IO/Embedded");
        var second = sut.GetOrCreate(TestAssembly, "IO/Embedded");

        // assert
        Assert.Same(first, second);
    }

    [Theory]
    [InlineData("IO/Embedded/")]
    [InlineData(@"IO\Embedded")]
    [InlineData("./IO/Embedded")]
    [InlineData("io/embedded")]
    public void GetOrCreate_ForEquivalentRoots_ReturnsCachedProvider(string equivalentRoot) {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var first = sut.GetOrCreate(TestAssembly, "IO/Embedded");
        var second = sut.GetOrCreate(TestAssembly, equivalentRoot);

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void GetOrCreate_ForSameAssemblyAndDifferentRoots_ReturnsDifferentProviders() {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var first = sut.GetOrCreate(TestAssembly, "IO");
        var second = sut.GetOrCreate(TestAssembly, "IO/Embedded");

        // assert
        Assert.Multiple(
            () => Assert.NotSame(first, second),
            () => Assert.Equal($"{ExpectedRoot}/IO", first.Root),
            () => Assert.Equal($"{ExpectedRoot}/IO/Embedded", second.Root)
        );
    }

    [Fact]
    public void GetOrCreate_OnDifferentFactoryInstances_DoesNotShareCache() {
        // act
        var first = new FileProviderFactory().GetOrCreate(TestAssembly);
        var second = new FileProviderFactory().GetOrCreate(TestAssembly);

        // assert
        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetOrCreate_WithNullAssembly_ThrowsArgumentNullException() {
        // arrange
        var sut = new FileProviderFactory();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetOrCreate(null!));
    }

    [Fact]
    public void GetOrCreate_WithNullRoot_ThrowsArgumentNullException() {
        // arrange
        var sut = new FileProviderFactory();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.GetOrCreate(TestAssembly, null!));
    }

    [Fact]
    public void GetOrCreate_WithRootNavigatingAboveManifestRoot_ThrowsRelativePathException() {
        // arrange
        var sut = new FileProviderFactory();

        // act & assert
        Assert.Throws<RelativePathException>(() => sut.GetOrCreate(TestAssembly, "../IO"));
    }

    [Fact]
    public void GetOrCreate_WithAssemblyWithoutManifest_ThrowsAndDoesNotCache() {
        // arrange
        var sut = new FileProviderFactory();

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
