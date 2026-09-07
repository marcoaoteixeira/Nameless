using System.Reactive.Linq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Reporting;

[UnitTest]
public class StatusMonitorTests {
    private static StatusReportingHub CreateHub() {
        return new StatusReportingHub(
            TimeProvider.System,
            OptionsHelper.Create<StatusReportingOptions>());
    }

    [Fact]
    public void ServiceName_WhenNoReporterCreatedYet_CreatesChannelAndReturnsName() {
        var hub = CreateHub();

        var sut = new StatusMonitor<SampleService>(hub);

        Assert.Equal("Nameless.Reporting.SampleService", sut.ServiceName);
    }

    [Fact]
    public void ServiceName_DelegatesToInnerMonitor() {
        var hub = CreateHub();
        hub.GetReporter(typeof(SampleService));

        var sut = new StatusMonitor<SampleService>(hub);

        Assert.Equal("Nameless.Reporting.SampleService", sut.ServiceName);
    }

    [Fact]
    public void ChannelKey_WhenNull_DelegatesToInnerMonitor() {
        var hub = CreateHub();
        hub.GetReporter(typeof(SampleService));

        var sut = new StatusMonitor<SampleService>(hub);

        Assert.Null(sut.ChannelKey);
    }

    [Fact]
    public void Status_DelegatesToInnerMonitor_ReceivesLiveUpdates() {
        var hub = CreateHub();
        var reporter = hub.GetReporter(typeof(SampleService));
        var sut = new StatusMonitor<SampleService>(hub);
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add);

        reporter.Report("live update", StatusLevel.Info, null);

        Assert.Contains(received, u => u.Message == "live update");
    }

    [Fact]
    public void Status_DelegatesToInnerMonitor_ReceivesReplayedHistory() {
        var hub = CreateHub();
        var reporter = hub.GetReporter(typeof(SampleService));
        reporter.Report("before subscribe", StatusLevel.Info, null);

        var sut = new StatusMonitor<SampleService>(hub);
        var received = new List<StatusUpdate>();
        sut.Status.Subscribe(received.Add);

        Assert.Contains(received, u => u.Message == "before subscribe");
    }

    [Fact]
    public void StatusMonitor_AndStatusReporter_ShareSameChannel_WhenMonitorCreatedFirst() {
        var hub = CreateHub();

        var monitor = new StatusMonitor<SampleService>(hub);
        var received = new List<StatusUpdate>();
        monitor.Status.Subscribe(received.Add);

        // reporter resolved after monitor — they must share the same channel
        var reporter = hub.GetReporter(typeof(SampleService));
        reporter.Report("after-monitor-creation", StatusLevel.Info, null);

        Assert.Contains(received, u => u.Message == "after-monitor-creation");
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private class SampleService { }
}
