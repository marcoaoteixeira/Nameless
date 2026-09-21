using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.IO.System;

namespace Nameless.Application;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    [Fact]
    public void RegisterApplicationContext_WithConfiguration_RegistersAndResolvesContext_WithBaseDataLocation() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();

        var inMemory = new Dictionary<string, string?> {
            ["ApplicationContext:ApplicationName"] = "svc.test.app",
            ["ApplicationContext:EnvironmentName"] = "svc-env",
            ["ApplicationContext:ApplicationDataLocation"] = "Base"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();

        // act
        var returned = services.RegisterApplicationContext(configuration);

        // assert returned collection is the same instance
        Assert.Same(services, returned);

        var provider = services.BuildServiceProvider();

        var ctx = provider.GetRequiredService<IApplicationContext>();
        Assert.Multiple(
            () => Assert.NotNull(ctx),
            () => Assert.Equal("svc.test.app", ctx.ApplicationName),
            () => Assert.Equal("svc-env", ctx.EnvironmentName)
        );

        // SystemFileExplorer should be created under AppDomain base directory when Base is used
        var fsp = ctx.ApplicationDataFileProvider as FileProvider;
        Assert.Multiple(
            () => Assert.NotNull(fsp),
            () => Assert.StartsWith(ctx.ApplicationDataDirectory, fsp.Root, StringComparison.OrdinalIgnoreCase),
            () => Assert.True(SysDirectory.Exists(fsp.Root))
        );
    }

    [Fact]
    public void RegisterApplicationContext_WithNullConfiguration_RegistersSingleton_IApplicationContext() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // act
        var returned = services.RegisterApplicationContext(configuration: null);

        // assert
        Assert.Same(services, returned);

        var provider = services.BuildServiceProvider();

        var a = provider.GetRequiredService<IApplicationContext>();
        var b = provider.GetRequiredService<IApplicationContext>();
        Assert.Same(a, b); // singleton
        Assert.IsType<ApplicationContext>(a);

        // Version should be at least set (default from options)
        Assert.False(string.IsNullOrWhiteSpace(a.Version));
    }
}
