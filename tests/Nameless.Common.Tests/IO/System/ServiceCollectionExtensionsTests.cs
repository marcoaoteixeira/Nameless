using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Nameless.IO.System;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    [Fact]
    public void RegisterFileProvider_ReturnsSameServiceCollection() {
        // arrange
        var services = new ServiceCollection();

        // act
        var actual = services.RegisterFileProvider();

        // assert
        Assert.Same(services, actual);
    }

    [Fact]
    public void RegisterFileProvider_ResolvesFileProviderRootedAtAppBaseDirectory() {
        // arrange
        var services = new ServiceCollection().RegisterFileProvider();
        using var provider = services.BuildServiceProvider();

        // act
        var actual = provider.GetRequiredService<IFileProvider>();

        // assert
        Assert.Multiple(
            () => Assert.IsType<FileProvider>(actual),
            () => Assert.Equal(
                SysPath.TrimEndingDirectorySeparator(SysPath.GetFullPath(AppContext.BaseDirectory)),
                SysPath.TrimEndingDirectorySeparator(actual.Root)
            )
        );
    }

    [Fact]
    public void RegisterFileProvider_RegistersFileProviderAsSingleton() {
        // arrange
        var services = new ServiceCollection().RegisterFileProvider();
        using var provider = services.BuildServiceProvider();

        // act
        var first = provider.GetRequiredService<IFileProvider>();
        var second = provider.GetRequiredService<IFileProvider>();

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void RegisterFileProvider_RegistersFileProviderFactoryAsSingleton() {
        // arrange
        var services = new ServiceCollection().RegisterFileProvider();
        using var provider = services.BuildServiceProvider();

        // act
        var first = provider.GetRequiredService<FileProviderFactory>();
        var second = provider.GetRequiredService<FileProviderFactory>();

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void RegisterFileProvider_ResolvedProviderComesFromRegisteredFactory() {
        // arrange
        var services = new ServiceCollection().RegisterFileProvider();
        using var provider = services.BuildServiceProvider();

        // act
        var actual = provider.GetRequiredService<IFileProvider>();
        var expected = provider.GetRequiredService<FileProviderFactory>()
                               .GetOrCreate(AppContext.BaseDirectory);

        // assert
        Assert.Same(expected, actual);
    }

    [Fact]
    public void RegisterFileProvider_WhenFileProviderAlreadyRegistered_DoesNotOverrideIt() {
        // arrange
        var existing = Mock.Of<IFileProvider>();
        var services = new ServiceCollection();
        services.AddSingleton(existing);

        // act
        services.RegisterFileProvider();
        using var provider = services.BuildServiceProvider();

        // assert
        Assert.Same(existing, provider.GetRequiredService<IFileProvider>());
    }

    [Fact]
    public void RegisterFileProvider_CalledTwice_DoesNotDuplicateRegistrations() {
        // arrange
        var services = new ServiceCollection();

        // act
        services.RegisterFileProvider()
                .RegisterFileProvider();

        // assert
        Assert.Multiple(
            () => Assert.Single(services, descriptor => descriptor.ServiceType == typeof(IFileProvider)),
            () => Assert.Single(services, descriptor => descriptor.ServiceType == typeof(FileProviderFactory))
        );
    }
}
