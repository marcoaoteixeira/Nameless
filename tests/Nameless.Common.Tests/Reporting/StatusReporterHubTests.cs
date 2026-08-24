using System.Reactive;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Reporting;

[UnitTest]
public class StatusReporterHubTests {
    // Stand-in for "TService" - the type a real BackgroundService/worker would use.
    private class FakeWorker { }

    [Fact]
    public void WhenGetOrCreate_CalledTwiceWithSameKey_ThenReturnsSameInstance() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();

        // act
        var first = hub.GetOrCreate("file1.txt");
        var second = hub.GetOrCreate("file1.txt");

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void WhenGetOrCreate_CalledWithDifferentKeys_ThenReturnsIndependentReporters() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();

        // act
        var first = hub.GetOrCreate("file1.txt");
        var second = hub.GetOrCreate("file2.txt");

        // assert
        Assert.NotSame(first, second);

        StatusUpdate? firstReceived = null;
        StatusUpdate? secondReceived = null;
        first.Status.Subscribe(update => firstReceived = update);
        second.Status.Subscribe(update => secondReceived = update);

        first.Report("From file1");

        Assert.Equal("From file1", firstReceived!.Message);
        Assert.Equal("Idle", secondReceived!.Message);
    }

    [Fact]
    public void WhenTryGet_ForUnknownKey_ThenReturnsFalse() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();

        // act
        var found = hub.TryGet("unknown", out var monitor);

        // assert
        Assert.False(found);
        Assert.Null(monitor);
    }

    [Fact]
    public void WhenTryGet_ForActiveChannelWithZeroConsumers_ThenStillReturnsIt() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();
        hub.GetOrCreate("file1.txt"); // created, never subscribed to

        // act
        var found = hub.TryGet("file1.txt", out var monitor);

        // assert
        Assert.True(found);
        Assert.NotNull(monitor);
    }

    [Fact]
    public void WhenChannelCompletes_WithNoActiveSubscribers_ThenIsEvictedImmediately() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();
        var reporter = hub.GetOrCreate("file1.txt");

        // act
        reporter.Complete();

        // assert
        var found = hub.TryGet("file1.txt", out _);
        Assert.False(found);
    }

    [Fact]
    public void WhenChannelCompletes_WithActiveSubscriber_ThenNotEvictedUntilSubscriberDisposes() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();
        var reporter = hub.GetOrCreate("file1.txt");

        // Subscribing via a raw IObserver<T> (rather than the Action<T>
        // convenience overloads) avoids Rx's AutoDetachObserver, which would
        // otherwise auto-dispose this subscription the instant it observes
        // OnCompleted/OnError - defeating the point of this test.
        var observer = Observer.Create<StatusUpdate>(_ => { });
        var subscription = reporter.Status.Subscribe(observer);

        // act
        reporter.Complete();

        // assert
        Assert.True(hub.TryGet("file1.txt", out _));

        subscription.Dispose();

        Assert.False(hub.TryGet("file1.txt", out _));
    }

    [Fact]
    public void WhenChannelFaults_ThenAlsoEvictedOnceIdle() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();
        var reporter = hub.GetOrCreate("file1.txt");
        reporter.Status.Subscribe(_ => { }, _ => { }, () => { }).Dispose();

        // act
        reporter.Fault("Something broke");

        // assert
        var found = hub.TryGet("file1.txt", out _);
        Assert.False(found);
    }

    [Fact]
    public void WhenGetOrCreate_CalledAfterEviction_ThenReturnsBrandNewEmptyChannel() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();
        var original = hub.GetOrCreate("file1.txt");
        original.Report("Old data");
        original.Complete();

        Assert.False(hub.TryGet("file1.txt", out _));

        // act
        var recreated = hub.GetOrCreate("file1.txt");

        // assert
        Assert.NotSame(original, recreated);

        var received = new List<StatusUpdate>();
        recreated.Status.Subscribe(received.Add);

        Assert.Single(received);
        Assert.Equal("Idle", received[0].Message);
    }

    [Fact]
    public async Task WhenMultipleFilesProcessedConcurrently_ThenEachChannelReportsIndependently() {
        // arrange
        var hub = new StatusReporterHub<FakeWorker>();

        var file1Updates = new List<StatusUpdate>();
        var file2Updates = new List<StatusUpdate>();

        var file1Reporter = hub.GetOrCreate("file1.txt");
        var file2Reporter = hub.GetOrCreate("file2.txt");

        file1Reporter.Status.Subscribe(file1Updates.Add);
        file2Reporter.Status.Subscribe(file2Updates.Add);

        var cancellationToken = TestContext.Current.CancellationToken;

        var file1Task = Task.Run(() => {
            for (var idx = 1; idx <= 5; idx++) {
                hub.GetOrCreate("file1.txt").Report($"file1 line {idx}");
            }
            hub.GetOrCreate("file1.txt").Complete();
        }, cancellationToken);

        var file2Task = Task.Run(() => {
            for (var idx = 1; idx <= 5; idx++) {
                hub.GetOrCreate("file2.txt").Report($"file2 line {idx}");
            }
            hub.GetOrCreate("file2.txt").Complete();
        }, cancellationToken);

        // act
        await Task.WhenAll(file1Task, file2Task);

        // assert
        Assert.All(file1Updates, update => Assert.StartsWith("FakeWorker#file1.txt", update.ServiceName));
        Assert.All(file2Updates, update => Assert.StartsWith("FakeWorker#file2.txt", update.ServiceName));
        Assert.Contains(file1Updates, update => update.Message == "file1 line 5");
        Assert.Contains(file2Updates, update => update.Message == "file2 line 5");
        Assert.DoesNotContain(file1Updates, update => update.Message.StartsWith("file2"));
        Assert.DoesNotContain(file2Updates, update => update.Message.StartsWith("file1"));
    }
}
