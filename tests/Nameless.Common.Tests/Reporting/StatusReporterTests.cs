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
    public void WhenStatus_SubscribedAfterMultipleReports_ThenReceivesAllBufferedValuesInOrder() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        reporter.Report("First");
        reporter.Report("Second");
        reporter.Report("Third");

        var received = new List<StatusUpdate>();

        // act
        reporter.Status.Subscribe(received.Add);

        // assert
        Assert.Equal(4, received.Count);
        Assert.Equal("Idle", received[0].Message);
        Assert.Equal("First", received[1].Message);
        Assert.Equal("Second", received[2].Message);
        Assert.Equal("Third", received[3].Message);
    }

    [Fact]
    public void WhenReportCount_ExceedsBufferSize_ThenOldestIsDropped() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>(bufferSize: 10);

        // act
        for (var idx = 1; idx <= 11; idx++) {
            reporter.Report($"Report {idx}");
        }

        var received = new List<StatusUpdate>();
        reporter.Status.Subscribe(received.Add);

        // assert
        Assert.Equal(10, received.Count);
        Assert.DoesNotContain(received, update => update.Message == "Report 1");
        Assert.Equal("Report 2", received[0].Message);
        Assert.Equal("Report 11", received[^1].Message);
    }

    [Fact]
    public void WhenChannelKey_Provided_ThenServiceNameIsComposedWithHash() {
        // arrange
        // act
        var reporter = new StatusReporter<FakeWorker>("orderplaced");

        // assert
        Assert.Equal("FakeWorker#orderplaced", reporter.ServiceName);
        Assert.Equal("orderplaced", reporter.ChannelKey);
    }

    [Fact]
    public void WhenChannelKey_Omitted_ThenServiceNameIsUnchanged() {
        // arrange
        // act
        var reporter = new StatusReporter<FakeWorker>();

        // assert
        Assert.Equal(nameof(FakeWorker), reporter.ServiceName);
        Assert.Equal(string.Empty, reporter.ChannelKey);
    }

    [Fact]
    public void WhenReport_Called_ThenTheUpdateIsTaggedWithTheReportersServiceName() {
        var reporter = new StatusReporter<FakeWorker>();
        StatusUpdate? received = null;
        reporter.Status.Subscribe(update => received = update);

        reporter.Report("Doing work");

        Assert.Equal(nameof(FakeWorker), received!.ServiceName);
    }

    [Fact]
    public void WhenComplete_Called_ThenSubscriberReceivesOnCompletedNotException() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        var completed = false;
        Exception? error = null;
        reporter.Status.Subscribe(_ => { }, ex => error = ex, () => completed = true);

        // act
        reporter.Complete();

        // assert
        Assert.True(completed);
        Assert.Null(error);
    }

    [Fact]
    public void WhenFault_Called_ThenSubscriberReceivesStatusFaultExceptionViaOnError() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        Exception? error = null;

        // act
        var exception = Record.Exception(() => {
            reporter.Status.Subscribe(_ => { }, ex => error = ex, () => { });
            reporter.Fault("Something broke", "ERR_001");
        });

        // assert
        Assert.Null(exception);
        var faultException = Assert.IsType<FaultException>(error);
        Assert.Equal("Something broke", faultException.Message);
        Assert.Equal("ERR_001", faultException.Code);
    }

    [Fact]
    public void WhenReport_CalledAfterComplete_ThenIsANoOp() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        var received = new List<StatusUpdate>();
        reporter.Status.Subscribe(received.Add);
        reporter.Complete();

        // act
        reporter.Report("Should not appear");

        // assert
        Assert.DoesNotContain(received, update => update.Message == "Should not appear");
    }

    [Fact]
    public void WhenSubscribe_AfterComplete_ThenImmediatelyReceivesBufferThenOnCompleted() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        reporter.Report("Last report");
        reporter.Complete();

        var received = new List<StatusUpdate>();
        var completed = false;

        // act
        reporter.Status.Subscribe(received.Add, () => completed = true);

        // assert
        Assert.Equal(["Idle", "Last report"], received.Select(update => update.Message));
        Assert.True(completed);
    }

    [Fact]
    public void WhenReport_CalledAfterDispose_ThenThrowsObjectDisposedException() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        reporter.Dispose();

        // act
        var exception = Record.Exception(() => reporter.Report("Should throw"));

        // assert
        Assert.IsType<ObjectDisposedException>(exception);
    }

    [Fact]
    public void WhenComplete_CalledAfterDispose_ThenThrowsObjectDisposedException() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        reporter.Dispose();

        // act
        var exception = Record.Exception(reporter.Complete);

        // assert
        Assert.IsType<ObjectDisposedException>(exception);
    }

    [Fact]
    public void WhenFault_CalledAfterDispose_ThenThrowsObjectDisposedException() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        reporter.Dispose();

        // act
        var exception = Record.Exception(() => reporter.Fault("Should throw"));

        // assert
        Assert.IsType<ObjectDisposedException>(exception);
    }

    [Fact]
    public void WhenStatus_SubscribedAfterDispose_ThenThrowsObjectDisposedException() {
        // arrange
        var reporter = new StatusReporter<FakeWorker>();
        reporter.Dispose();

        // act
        var exception = Record.Exception(() => reporter.Status.Subscribe(_ => { }));

        // assert
        Assert.IsType<ObjectDisposedException>(exception);
    }
}
