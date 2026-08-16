using Nameless.Testing.Tools.Attributes;

namespace Nameless.Reporting;

[UnitTest]
public class StatusReporterTests {
    // Stand-in for "TService" - the type a real BackgroundService/worker would use.
    private class FakeWorker { }

    [Fact]
    public void WhenServiceName_Accessed_ThenDefaultsToTheGenericTypeName() {
        var reporter = new StatusReporter<FakeWorker>();

        Assert.Equal(nameof(FakeWorker), reporter.ServiceName);
    }

    [Fact]
    public void WhenStatus_SubscribedBeforeAnyReport_ThenImmediatelyReceivesAnIdleValue() {
        var reporter = new StatusReporter<FakeWorker>();
        StatusUpdate? received = null;

        reporter.Status.Subscribe(update => received = update);

        Assert.NotNull(received);
        Assert.Equal("Idle", received!.Message);
        Assert.Equal(StatusLevel.Info, received.Level);
    }

    [Fact]
    public void WhenReport_CalledWithAnExistingSubscriber_ThenPushesANewValue() {
        var reporter = new StatusReporter<FakeWorker>();
        var received = new List<StatusUpdate>();
        reporter.Status.Subscribe(received.Add);

        reporter.Report("Processing batch 1");

        Assert.Equal(2, received.Count); // initial Idle + the report
        Assert.Equal("Processing batch 1", received[^1].Message);
    }

    [Fact]
    public void WhenReport_CalledWithoutAnExplicitLevel_ThenDefaultsToInfo() {
        var reporter = new StatusReporter<FakeWorker>();
        StatusUpdate? received = null;
        reporter.Status.Subscribe(update => received = update);

        reporter.Report("Just chugging along");

        Assert.Equal(StatusLevel.Info, received!.Level);
    }

    [Fact]
    public void WhenReport_CalledWithAnExplicitLevel_ThenHonorsThatLevel() {
        var reporter = new StatusReporter<FakeWorker>();
        StatusUpdate? received = null;
        reporter.Status.Subscribe(update => received = update);

        reporter.Report("Something went wrong", StatusLevel.Error);

        Assert.Equal(StatusLevel.Error, received!.Level);
    }

    [Fact]
    public void WhenStatus_SubscribedAfterMultipleReports_ThenImmediatelyReceivesOnlyTheMostRecentValue() {
        var reporter = new StatusReporter<FakeWorker>();
        reporter.Report("First");
        reporter.Report("Second");
        reporter.Report("Third");

        var received = new List<StatusUpdate>();
        reporter.Status.Subscribe(received.Add);

        Assert.Single(received);
        Assert.Equal("Third", received[0].Message);
    }

    [Fact]
    public void WhenReport_Called_ThenTheUpdateIsTaggedWithTheReportersServiceName() {
        var reporter = new StatusReporter<FakeWorker>();
        StatusUpdate? received = null;
        reporter.Status.Subscribe(update => received = update);

        reporter.Report("Doing work");

        Assert.Equal(nameof(FakeWorker), received!.ServiceName);
    }
}