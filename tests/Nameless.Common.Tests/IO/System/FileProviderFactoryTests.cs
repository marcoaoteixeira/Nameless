namespace Nameless.IO.System;

[UnitTest]
public class FileProviderFactoryTests {
    private static readonly string Root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");

    [Fact]
    public void GetOrCreate_ReturnsProviderForRoot() {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var actual = sut.GetOrCreate(Root);

        // assert
        Assert.Equal($"{Root}{SysPath.DirectorySeparatorChar}", actual.Root);
    }

    [Fact]
    public void GetOrCreate_ForSameRoot_ReturnsCachedProvider() {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var first = sut.GetOrCreate(Root);
        var second = sut.GetOrCreate(Root);

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void GetOrCreate_ForDifferentRoots_ReturnsDifferentProviders() {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var first = sut.GetOrCreate(Root);
        var second = sut.GetOrCreate(SysPath.Combine(Root, "other"));

        // assert
        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetOrCreate_OnDifferentFactoryInstances_DoesNotShareCache() {
        // act
        var first = new FileProviderFactory().GetOrCreate(Root);
        var second = new FileProviderFactory().GetOrCreate(Root);

        // assert
        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetOrCreate_ForRootDifferingOnlyByCase_OnWindows_ReturnsCachedProvider() {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Paths are case-insensitive only on Windows.");

        // arrange
        var sut = new FileProviderFactory();

        // act
        var first = sut.GetOrCreate(Root.ToLowerInvariant());
        var second = sut.GetOrCreate(Root.ToUpperInvariant());

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void GetOrCreate_WithRelativeRoot_ThrowsArgumentException() {
        // arrange
        var sut = new FileProviderFactory();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.GetOrCreate("relative/root"));
    }
}
