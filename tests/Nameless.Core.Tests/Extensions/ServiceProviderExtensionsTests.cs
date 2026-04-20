using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless;

public class ServiceProviderExtensionsTests {
    // --- GetLogger<T> ---

    [Fact]
    public void GetLogger_WhenLoggerFactoryRegistered_ReturnsLogger() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var provider = services.BuildServiceProvider();

        // act
        var logger = provider.GetLogger<ServiceProviderExtensionsTests>();

        // assert
        Assert.NotNull(logger);
    }

    [Fact]
    public void GetLogger_WhenNoLoggerFactory_ReturnsNullLogger() {
        // arrange
        var provider = new ServiceCollection().BuildServiceProvider();

        // act
        var logger = provider.GetLogger<ServiceProviderExtensionsTests>();

        // assert
        Assert.Same(NullLogger<ServiceProviderExtensionsTests>.Instance, logger);
    }

    [Fact]
    public void GetLogger_ByType_WhenNoLoggerFactory_ReturnsNullLogger() {
        // arrange
        var provider = new ServiceCollection().BuildServiceProvider();

        // act
        var logger = provider.GetLogger(typeof(ServiceProviderExtensionsTests));

        // assert
        Assert.Same(NullLogger.Instance, logger);
    }

    // --- GetOptions ---

    [Fact]
    public void GetOptions_WhenRegisteredInDI_ReturnsRegisteredOptions() {
        // arrange
        var services = new ServiceCollection();
        services.Configure<SampleOptions>(o => o.Value = 42);
        var provider = services.BuildServiceProvider();

        // act
        var options = provider.GetOptions<SampleOptions>();

        // assert
        Assert.Equal(42, options.Value.Value);
    }

    [Fact]
    public void GetOptions_WhenFromConfiguration_ReturnsConfigOptions() {
        // arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["SampleOptions:Value"] = "99"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        var provider = services.BuildServiceProvider();

        // act
        var options = provider.GetOptions<SampleOptions>();

        // assert
        Assert.Equal(99, options.Value.Value);
    }

    [Fact]
    public void GetOptions_WhenFromFactory_ReturnsFactoryOptions() {
        // arrange
        var provider = new ServiceCollection().BuildServiceProvider();

        // act
        var options = provider.GetOptions<SampleOptions>(() => new SampleOptions { Value = 7 });

        // assert
        Assert.Equal(7, options.Value.Value);
    }

    [Fact]
    public void GetOptions_WhenNothingConfigured_ReturnsDefaultInstance() {
        // arrange
        var provider = new ServiceCollection().BuildServiceProvider();

        // act
        var options = provider.GetOptions<SampleOptions>();

        // assert
        Assert.NotNull(options.Value);
        Assert.Equal(0, options.Value.Value);
    }

    // --- test doubles ---

    private sealed class SampleOptions {
        public int Value { get; set; }
    }
}
