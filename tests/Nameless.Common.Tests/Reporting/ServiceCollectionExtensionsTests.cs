using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Reporting;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    private static IServiceCollection BuildServices(IConfiguration? configuration = null) {
        var services = new ServiceCollection();
        services.AddSingleton(TimeProvider.System);
        services.RegisterStatusReporting(configuration ?? new ConfigurationBuilder().Build());
        return services;
    }

    [Fact]
    public void RegisterStatusReporting_ReturnsSameServiceCollection() {
        var services = new ServiceCollection();
        services.AddSingleton(TimeProvider.System);
        var config = new ConfigurationBuilder().Build();

        var returned = services.RegisterStatusReporting(config);

        Assert.Same(services, returned);
    }

    [Fact]
    public void RegisterStatusReporting_Registers_IStatusReportingHub_AsSingleton() {
        var provider = BuildServices().BuildServiceProvider();

        var a = provider.GetRequiredService<IStatusReportingHub>();
        var b = provider.GetRequiredService<IStatusReportingHub>();

        Assert.Same(a, b);
        Assert.IsType<StatusReportingHub>(a);
    }

    [Fact]
    public void RegisterStatusReporting_Registers_IStatusReporter_Generic_AsSingleton() {
        var provider = BuildServices().BuildServiceProvider();

        var a = provider.GetRequiredService<IStatusReporter<SampleWorker>>();
        var b = provider.GetRequiredService<IStatusReporter<SampleWorker>>();

        Assert.Same(a, b);
        Assert.IsType<StatusReporter<SampleWorker>>(a);
    }

    [Fact]
    public void RegisterStatusReporting_Registers_IStatusMonitor_Generic_AsSingleton() {
        var provider = BuildServices().BuildServiceProvider();

        var a = provider.GetRequiredService<IStatusMonitor<SampleWorker>>();
        var b = provider.GetRequiredService<IStatusMonitor<SampleWorker>>();

        Assert.Same(a, b);
        Assert.IsType<StatusMonitor<SampleWorker>>(a);
    }

    [Fact]
    public void RegisterStatusReporting_IsIdempotent() {
        var services = new ServiceCollection();
        services.AddSingleton(TimeProvider.System);
        var config = new ConfigurationBuilder().Build();
        services.RegisterStatusReporting(config);
        services.RegisterStatusReporting(config);

        var provider = services.BuildServiceProvider();
        var a = provider.GetRequiredService<IStatusReportingHub>();
        var b = provider.GetRequiredService<IStatusReportingHub>();

        Assert.Same(a, b);
    }

    [Fact]
    public void RegisterStatusReporting_BindsBufferSize_FromConfiguration() {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["StatusReporting:BufferSize"] = "25"
            })
            .Build();

        var provider = BuildServices(config).BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<StatusReportingOptions>>();

        Assert.Equal(25, options.Value.BufferSize);
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private class SampleWorker { }
}
