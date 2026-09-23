using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.IO.System;

namespace Nameless.IO;

[IntegrationTest]
public class DirectoryHelperTests : IDisposable {
    private readonly string _root = SysPath.Combine(SysPath.GetTempPath(), $"dirhelper-{Guid.NewGuid():N}");

    public DirectoryHelperTests() {
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        SysDirectory.Delete(_root, recursive: true);
    }

    [Fact]
    public void CopyDirectory_CopiesFilesAndNestedDirectories() {
        // arrange
        var source = SysPath.Combine(_root, "src");
        var nested = SysPath.Combine(source, "nested");
        SysDirectory.CreateDirectory(nested);
        SysFile.WriteAllText(SysPath.Combine(source, "a.txt"), "a");
        SysFile.WriteAllText(SysPath.Combine(nested, "b.txt"), "b");
        var destination = SysPath.Combine(_root, "dst");

        // act
        DirectoryHelper.CopyDirectory(source, destination, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.Equal("a", SysFile.ReadAllText(SysPath.Combine(destination, "a.txt"))),
            () => Assert.Equal("b", SysFile.ReadAllText(SysPath.Combine(destination, "nested", "b.txt")))
        );
    }

    [Fact]
    public void CopyDirectory_OverwritesExistingFiles() {
        // arrange
        var source = SysPath.Combine(_root, "src");
        var destination = SysPath.Combine(_root, "dst");
        SysDirectory.CreateDirectory(source);
        SysDirectory.CreateDirectory(destination);
        SysFile.WriteAllText(SysPath.Combine(source, "a.txt"), "new");
        SysFile.WriteAllText(SysPath.Combine(destination, "a.txt"), "old");

        // act
        DirectoryHelper.CopyDirectory(source, destination, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal("new", SysFile.ReadAllText(SysPath.Combine(destination, "a.txt")));
    }

    [Fact]
    public void CopyDirectory_WithMissingSource_Throws() {
        // act & assert
        Assert.Throws<DirectoryNotFoundException>(() => DirectoryHelper.CopyDirectory(SysPath.Combine(_root, "none"), SysPath.Combine(_root, "dst"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public void CopyDirectory_WithCancelledToken_Throws() {
        // arrange
        var source = SysPath.Combine(_root, "src");
        SysDirectory.CreateDirectory(source);
        SysFile.WriteAllText(SysPath.Combine(source, "a.txt"), "a");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // act & assert
        Assert.Throws<OperationCanceledException>(() => DirectoryHelper.CopyDirectory(source, SysPath.Combine(_root, "dst"), cts.Token));
    }

    [Fact]
    public void FileProviderFactory_ForSameRoot_ReturnsCachedProvider() {
        // arrange
        var sut = new FileProviderFactory();

        // act
        var first = sut.GetOrCreate(options => options.Root = _root);
        var second = sut.GetOrCreate(options => options.Root = _root);

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void FileProviderFactory_ForDifferentRoots_ReturnsDifferentProviders() {
        // arrange
        var sut = new FileProviderFactory();
        var other = SysPath.Combine(_root, "other");
        SysDirectory.CreateDirectory(other);

        // act
        var first = sut.GetOrCreate(options => options.Root = _root);
        var second = sut.GetOrCreate(options => options.Root = other);

        // assert
        Assert.NotSame(first, second);
    }

    [Fact]
    public void RegisterFileProvider_UsesConfiguredRoot() {
        // arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["FileProvider:Root"] = _root,
                ["FileProvider:AllowOperationOutsideRoot"] = "true"
            })
            .Build();

        var services = new ServiceCollection();

        // act
        var returned = services.RegisterFileProvider(configuration);
        using var provider = services.BuildServiceProvider();
        var fileProvider = provider.GetRequiredService<IFileProvider>();

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.Equal(PathHelper.Normalize(_root), PathHelper.Normalize(fileProvider.Root).TrimEnd(SysPath.DirectorySeparatorChar)),
            () => Assert.Same(fileProvider, provider.GetRequiredService<IFileProvider>())
        );
    }
}
