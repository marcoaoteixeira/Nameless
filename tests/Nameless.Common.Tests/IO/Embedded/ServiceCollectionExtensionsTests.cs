using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nameless.IO.System;

namespace Nameless.IO.Embedded;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    private static readonly Assembly TestAssembly = typeof(ServiceCollectionExtensionsTests).Assembly;
    private static readonly string? ServiceKey = TestAssembly.GetName().Name;

    [Fact]
    public void RegisterEmbeddedFileProvider_ReturnsSameServiceCollection() {
        // arrange
        var services = new ServiceCollection();

        // act
        var actual = services.RegisterEmbeddedFileProvider(TestAssembly);

        // assert
        Assert.Same(services, actual);
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_WithNullAssembly_ThrowsArgumentNullException() {
        // arrange
        var services = new ServiceCollection();

        // act & assert
        Assert.Throws<ArgumentNullException>(() => services.RegisterEmbeddedFileProvider(null!));
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_ResolvesKeyedEmbeddedFileProviderByAssemblyName() {
        // arrange
        using var provider = new ServiceCollection().RegisterEmbeddedFileProvider(TestAssembly)
                                                    .BuildServiceProvider();

        // act
        var actual = provider.GetRequiredKeyedService<IFileProvider>(ServiceKey);

        // assert
        Assert.Multiple(
            () => Assert.IsType<EmbeddedFileProvider>(actual),
            () => Assert.Equal($"embedded://{ServiceKey}/", actual.Root)
        );
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_RegistersProviderAsSingleton() {
        // arrange
        using var provider = new ServiceCollection().RegisterEmbeddedFileProvider(TestAssembly)
                                                    .BuildServiceProvider();

        // act
        var first = provider.GetRequiredKeyedService<IFileProvider>(ServiceKey);
        var second = provider.GetRequiredKeyedService<IFileProvider>(ServiceKey);

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_ResolvedProviderComesFromRegisteredFactory() {
        // arrange
        using var provider = new ServiceCollection().RegisterEmbeddedFileProvider(TestAssembly)
                                                    .BuildServiceProvider();

        // act
        var actual = provider.GetRequiredKeyedService<IFileProvider>(ServiceKey);
        var expected = provider.GetRequiredService<EmbeddedFileProviderFactory>()
                               .GetOrCreate(TestAssembly);

        // assert
        Assert.Same(expected, actual);
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_DoesNotRegisterNonKeyedFileProvider() {
        // arrange
        using var provider = new ServiceCollection().RegisterEmbeddedFileProvider(TestAssembly)
                                                    .BuildServiceProvider();

        // act
        var actual = provider.GetService<IFileProvider>();

        // assert
        Assert.Null(actual);
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_WithPhysicalProviderRegistered_KeepsBothResolvable() {
        // arrange
        using var provider = new ServiceCollection().RegisterFileProvider()
                                                    .RegisterEmbeddedFileProvider(TestAssembly)
                                                    .BuildServiceProvider();

        // act
        var physical = provider.GetRequiredService<IFileProvider>();
        var embedded = provider.GetRequiredKeyedService<IFileProvider>(ServiceKey);

        // assert
        Assert.Multiple(
            () => Assert.IsType<FileProvider>(physical),
            () => Assert.IsType<EmbeddedFileProvider>(embedded)
        );
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_CalledTwice_DoesNotDuplicateRegistrations() {
        // arrange
        var services = new ServiceCollection();

        // act
        services.RegisterEmbeddedFileProvider(TestAssembly)
                .RegisterEmbeddedFileProvider(TestAssembly);

        // assert
        Assert.Multiple(
            () => Assert.Single(services, descriptor => descriptor.ServiceType == typeof(IFileProvider)),
            () => Assert.Single(services, descriptor => descriptor.ServiceType == typeof(EmbeddedFileProviderFactory))
        );
    }

    [Fact]
    public void RegisterEmbeddedFileProvider_ForDifferentAssemblies_RegistersOneKeyPerAssembly() {
        // arrange
        var other = typeof(Throws).Assembly;
        var services = new ServiceCollection();

        // act
        services.RegisterEmbeddedFileProvider(TestAssembly)
                .RegisterEmbeddedFileProvider(other);

        // assert
        var keys = services.Where(descriptor => descriptor.ServiceType == typeof(IFileProvider))
                           .Select(descriptor => descriptor.ServiceKey)
                           .ToArray();

        Assert.Equal([ServiceKey, other.GetName().Name], keys);
    }
}
