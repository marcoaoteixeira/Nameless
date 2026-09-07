using System.Reactive.Linq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Reporting;

[UnitTest]
public class StatusReportingHubTests {
    private static StatusReportingHub CreateHub(int bufferSize = 10) {
        return new StatusReportingHub(
            TimeProvider.System,
            OptionsHelper.Create<StatusReportingOptions>(o => o.BufferSize = bufferSize));
    }

    // ─── GetReporter ─────────────────────────────────────────────────────────

    [Fact]
    public void GetReporter_WithNullService_Throws() {
        var hub = CreateHub();

        Assert.Throws<ArgumentNullException>(() => hub.GetReporter(null!));
    }

    [Fact]
    public void GetReporter_ReturnsReporter() {
        var hub = CreateHub();

        var reporter = hub.GetReporter(typeof(SampleService));

        Assert.NotNull(reporter);
    }

    [Fact]
    public void GetReporter_ReturnsSameInstance_ForSameServiceAndChannel() {
        var hub = CreateHub();

        var first = hub.GetReporter(typeof(SampleService), "ch-1");
        var second = hub.GetReporter(typeof(SampleService), "ch-1");

        Assert.Same(first, second);
    }

    [Fact]
    public void GetReporter_ReturnsDifferentInstances_ForDifferentChannels() {
        var hub = CreateHub();

        var first = hub.GetReporter(typeof(SampleService), "ch-1");
        var second = hub.GetReporter(typeof(SampleService), "ch-2");

        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetReporter_ReturnsDifferentInstances_ForDifferentServiceTypes() {
        var hub = CreateHub();

        var first = hub.GetReporter(typeof(SampleService));
        var second = hub.GetReporter(typeof(OtherService));

        Assert.NotSame(first, second);
    }

    // ─── GetMonitor ───────────────────────────────────────────────────────────

    [Fact]
    public void GetMonitor_WithNullService_Throws() {
        var hub = CreateHub();

        Assert.Throws<ArgumentNullException>(() => hub.GetMonitor(null!));
    }

    [Fact]
    public void GetMonitor_ReturnsMonitor() {
        var hub = CreateHub();

        var monitor = hub.GetMonitor(typeof(SampleService));

        Assert.NotNull(monitor);
    }

    [Fact]
    public void GetMonitor_ReturnsSameInstance_ForSameServiceAndChannel() {
        var hub = CreateHub();

        var first = hub.GetMonitor(typeof(SampleService), "ch-1");
        var second = hub.GetMonitor(typeof(SampleService), "ch-1");

        Assert.Same(first, second);
    }

    [Fact]
    public void GetMonitor_ReturnsDifferentInstances_ForDifferentChannels() {
        var hub = CreateHub();

        var first = hub.GetMonitor(typeof(SampleService), "ch-1");
        var second = hub.GetMonitor(typeof(SampleService), "ch-2");

        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetMonitor_ReturnsDifferentInstances_ForDifferentServiceTypes() {
        var hub = CreateHub();

        var first = hub.GetMonitor(typeof(SampleService));
        var second = hub.GetMonitor(typeof(OtherService));

        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetMonitor_ReturnsCorrectServiceName() {
        var hub = CreateHub();

        var monitor = hub.GetMonitor(typeof(SampleService));

        Assert.Equal("Nameless.Reporting.SampleService", monitor.ServiceName);
    }

    [Fact]
    public void GetMonitor_AndGetReporter_ShareSameChannel() {
        var hub = CreateHub();

        var reporter = hub.GetReporter(typeof(SampleService));
        var monitor = hub.GetMonitor(typeof(SampleService));

        Assert.Same(reporter, monitor);
    }

    [Fact]
    public void GetMonitor_BeforeGetReporter_SharesSameChannel() {
        var hub = CreateHub();

        var monitor = hub.GetMonitor(typeof(SampleService)); // called first
        var reporter = hub.GetReporter(typeof(SampleService));

        Assert.Same(monitor, reporter);
    }

    // ─── Eviction ─────────────────────────────────────────────────────────────

    [Fact]
    public void AfterComplete_WithNoSubscribers_ChannelIsEvicted() {
        var hub = CreateHub();
        var original = hub.GetReporter(typeof(SampleService));

        original.Complete();

        Assert.NotSame(original, hub.GetReporter(typeof(SampleService)));
    }

    [Fact]
    public void AfterComplete_ChannelStillRegistered_WhileSubscriberActive() {
        var hub = CreateHub();
        var reporter = hub.GetReporter(typeof(SampleService));
        var monitor = hub.GetMonitor(typeof(SampleService));

        var sub = monitor.Status.Subscribe(_ => { }, () => { });
        reporter.Complete();

        Assert.Same(reporter, hub.GetReporter(typeof(SampleService)));

        sub.Dispose();
    }

    [Fact]
    public void AfterComplete_AndAllSubscribersDispose_ChannelIsEvicted() {
        var hub = CreateHub();
        var reporter = hub.GetReporter(typeof(SampleService));
        var monitor = hub.GetMonitor(typeof(SampleService));

        var sub = monitor.Status.Subscribe(_ => { }, () => { });
        reporter.Complete();
        sub.Dispose();

        Assert.NotSame(reporter, hub.GetReporter(typeof(SampleService)));
    }

    [Fact]
    public void AfterFault_AndAllSubscribersDispose_ChannelIsEvicted() {
        var hub = CreateHub();
        var reporter = hub.GetReporter(typeof(SampleService));
        var monitor = hub.GetMonitor(typeof(SampleService));

        var sub = monitor.Status.Subscribe(_ => { }, _ => { });
        reporter.Fault("boom", null);
        sub.Dispose();

        Assert.NotSame(reporter, hub.GetReporter(typeof(SampleService)));
    }

    [Fact]
    public void AfterEviction_GetReporter_CreatesNewChannel() {
        var hub = CreateHub();
        var first = hub.GetReporter(typeof(SampleService));

        first.Complete(); // evicts immediately (no subscribers)

        var second = hub.GetReporter(typeof(SampleService));

        Assert.NotSame(first, second);
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private class SampleService { }
    private class OtherService { }
}
