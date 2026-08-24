using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Reporting;

public class ServiceCollectionExtensionsTests {
    private class FakeWorker { }

    private static ServiceProvider BuildProvider() {
        var services = new ServiceCollection();
        services.RegisterStatusReporting<FakeWorker>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void WhenIStatusReporterAndIStatusMonitor_BothResolved_ThenTheyReturnTheSameUnderlyingInstance() {
        using var provider = BuildProvider();

        var reporter = provider.GetRequiredService<IStatusReporter<FakeWorker>>();
        var monitor = provider.GetRequiredService<IStatusMonitor<FakeWorker>>();

        StatusUpdate? received = null;
        monitor.Status.Subscribe(update => received = update);

        reporter.Report("Hello from the reporter");

        Assert.Equal("Hello from the reporter", received!.Message);
    }

    [Fact]
    public void WhenIStatusReporter_ResolvedTwice_ThenReturnsTheSameSingletonInstance() {
        using var provider = BuildProvider();

        var first = provider.GetRequiredService<IStatusReporter<FakeWorker>>();
        var second = provider.GetRequiredService<IStatusReporter<FakeWorker>>();

        Assert.Same(first, second);
    }

    [Fact]
    public void WhenIStatusMonitor_ResolvedTwice_ThenReturnsTheSameSingletonInstance() {
        using var provider = BuildProvider();

        var first = provider.GetRequiredService<IStatusMonitor<FakeWorker>>();
        var second = provider.GetRequiredService<IStatusMonitor<FakeWorker>>();

        Assert.Same(first, second);
    }

    [Fact]
    public void WhenConcreteStatusReporterType_ResolvedWithoutItsKey_ThenIsNotReachable() {
        using var provider = BuildProvider();

        var concrete = provider.GetService<StatusReporter<FakeWorker>>();

        Assert.Null(concrete);
    }

    [Fact]
    public void WhenRegisterStatusReporter_CalledTwiceForTheSameServiceType_ThenItDoesNotThrowAndKeepsIdentity() {
        var services = new ServiceCollection();
        services.RegisterStatusReporting<FakeWorker>();
        services.RegisterStatusReporting<FakeWorker>(); // idempotent call, e.g. from two setup paths

        using var provider = services.BuildServiceProvider();

        var reporter = provider.GetRequiredService<IStatusReporter<FakeWorker>>();
        var monitor = provider.GetRequiredService<IStatusMonitor<FakeWorker>>();

        StatusUpdate? received = null;
        monitor.Status.Subscribe(update => received = update);
        reporter.Report("Still wired correctly");

        Assert.Equal("Still wired correctly", received!.Message);
    }

    [Fact]
    public void WhenRegisterStatusReporter_CalledForDifferentServiceTypes_ThenTheirReportersAreIndependent() {
        var services = new ServiceCollection();
        services.RegisterStatusReporting<FakeWorker>();
        services.RegisterStatusReporting<AnotherFakeWorker>();
        using var provider = services.BuildServiceProvider();

        var workerReporter = provider.GetRequiredService<IStatusReporter<FakeWorker>>();
        _ = provider.GetRequiredService<IStatusReporter<AnotherFakeWorker>>();

        workerReporter.Report("Worker-specific message");

        StatusUpdate? anotherReceived = null;

        provider
            .GetRequiredService<IStatusMonitor<AnotherFakeWorker>>()
            .Status
            .Subscribe(update => anotherReceived = update);

        Assert.Equal("Idle", anotherReceived!.Message); // untouched by the other worker's report
    }

    [Fact]
    public void WhenRegisterStatusReporterHub_Resolved_ThenGetOrCreateProducesIndependentChannelsPerKey() {
        var services = new ServiceCollection();
        services.RegisterStatusReporting<FakeWorker>();
        using var provider = services.BuildServiceProvider();

        var hub = provider.GetRequiredService<IStatusReporterHub<FakeWorker>>();
        var monitorHub = provider.GetRequiredService<IStatusMonitorHub<FakeWorker>>();

        var file1 = hub.GetOrCreate("file1.txt");
        var file2 = hub.GetOrCreate("file2.txt");

        Assert.NotSame(file1, file2);

        StatusUpdate? received = null;
        file1.Report("From file1");

        Assert.True(monitorHub.TryGet("file1.txt", out var monitor));
        monitor!.Status.Subscribe(update => received = update);

        Assert.Equal("From file1", received!.Message);
    }

    [Fact]
    public void WhenConcreteStatusReporterHubType_ResolvedWithoutItsKey_ThenIsNotReachable() {
        var services = new ServiceCollection();
        services.RegisterStatusReporting<FakeWorker>();
        using var provider = services.BuildServiceProvider();

        var concrete = provider.GetService<StatusReporterHub<FakeWorker>>();

        Assert.Null(concrete);
    }

    private class AnotherFakeWorker { }
}