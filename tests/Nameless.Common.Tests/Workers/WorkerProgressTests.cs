using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Null;
using Nameless.Workers.Notification;

namespace Nameless.Workers;

public class WorkerProgressTests {
    private static IConfiguration CreateConfiguration(string workerName, string interval = "00:00:00.050") {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                [$"Workers:{workerName}:IsEnabled"] = "true",
                [$"Workers:{workerName}:Interval"] = interval,
            })
            .Build();
    }

    // ─── LastProgress ──────────────────────────────────────────────────────

    [Fact]
    public void LastProgress_IsNull_BeforeAnyProgressIsReported() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        // assert
        Assert.Null(worker.LastProgress);

        worker.Dispose();
    }

    [Fact]
    public async Task LastProgress_IsSet_AfterReportProgress() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var progressReportedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            self => {
                self.ReportInformation("step one", percentageComplete: 50);
                progressReportedTcs.TrySetResult();
                return Task.CompletedTask;
            }
        );

        await worker.StartAsync(ct);
        await progressReportedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // assert
        Assert.NotNull(worker.LastProgress);
        Assert.Equal("step one", worker.LastProgress!.Message);
        Assert.Equal(50, worker.LastProgress.PercentageComplete);

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task LastProgress_ReflectsLatestNotification() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var thirdTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            self => {
                self.ReportInformation("first");
                self.ReportInformation("second");
                self.ReportInformation("third");
                thirdTcs.TrySetResult();
                return Task.CompletedTask;
            }
        );

        await worker.StartAsync(ct);
        await thirdTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // assert
        Assert.Equal("third", worker.LastProgress?.Message);

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    // ─── IObservable / Subscribe ──────────────────────────────────────────

    [Fact]
    public async Task Progress_Subscribe_ReceivesAllReportedItems() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var received = new List<WorkerProgress>();
        var doneTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            self => {
                self.ReportInformation("a");
                self.ReportInformation("b");
                self.ReportInformation("c");
                doneTcs.TrySetResult();
                return Task.CompletedTask;
            }
        );

        using (worker.Progress.Subscribe(new CollectingObserver(received))) {
            await worker.StartAsync(ct);
            await doneTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        }

        // assert: at minimum the three items we published
        Assert.Contains(received, p => p.Message == "a");
        Assert.Contains(received, p => p.Message == "b");
        Assert.Contains(received, p => p.Message == "c");

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task Progress_DisposedSubscription_StopsReceivingItems() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var received = new List<WorkerProgress>();
        var firstTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            async self => {
                self.ReportInformation("before-unsubscribe");
                firstTcs.TrySetResult();
                await releaseTcs.Task;
                self.ReportInformation("after-unsubscribe");
            }
        );

        var subscription = worker.Progress.Subscribe(new CollectingObserver(received));
        await worker.StartAsync(ct);
        await firstTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // unsubscribe before second report
        subscription.Dispose();
        releaseTcs.TrySetResult();
        await Task.Delay(100, ct); // let the second report fire

        // assert: only the first item arrived
        Assert.Contains(received, p => p.Message == "before-unsubscribe");
        Assert.DoesNotContain(received, p => p.Message == "after-unsubscribe");

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task StopAsync_CompletesObservable_ForSubscribers() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var completedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        using (worker.Progress.Subscribe(new CompletionObserver(completedTcs))) {
            await worker.StartAsync(ct);

            // act
            await worker.StopAsync(ct);
        }

        // assert
        await completedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        Assert.True(completedTcs.Task.IsCompletedSuccessfully);

        worker.Dispose();
    }

    [Fact]
    public async Task Progress_SubscriberThrowingOnNext_DoesNotBlockOtherSubscribers() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var goodReceived = new List<WorkerProgress>();
        var doneTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            self => {
                self.ReportInformation("item");
                doneTcs.TrySetResult();
                return Task.CompletedTask;
            }
        );

        // bad subscriber that always throws in OnNext
        worker.Progress.Subscribe(new ThrowingObserver());
        // good subscriber that collects items
        worker.Progress.Subscribe(new CollectingObserver(goodReceived));

        await worker.StartAsync(ct);
        await doneTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // assert: good subscriber still received the item despite bad subscriber
        Assert.Contains(goodReceived, p => p.Message == "item");

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    // ─── Extension methods ────────────────────────────────────────────────

    [Fact]
    public void ReportTickStarted_SetsCorrectTypeAndZeroPercent() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        // act
        worker.ReportTickStarted();

        // assert
        Assert.Equal(WorkerProgressType.TickStarted, worker.LastProgress?.Type);
        Assert.Equal(0, worker.LastProgress?.PercentageComplete);

        worker.Dispose();
    }

    [Fact]
    public void ReportTickCompleted_SetsCorrectTypeAndHundredPercent() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        // act
        worker.ReportTickCompleted();

        // assert
        Assert.Equal(WorkerProgressType.TickCompleted, worker.LastProgress?.Type);
        Assert.Equal(100, worker.LastProgress?.PercentageComplete);

        worker.Dispose();
    }

    [Fact]
    public void ReportTickFailed_SetsCorrectTypeAndMetadata() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );
        var ex = new InvalidOperationException("test error");

        // act
        worker.ReportTickFailed(ex);

        // assert
        Assert.Equal(WorkerProgressType.TickFailed, worker.LastProgress?.Type);
        Assert.NotNull(worker.LastProgress?.Metadata);
        Assert.Equal("test error", worker.LastProgress!.Metadata!["Exception"]);

        worker.Dispose();
    }

    // ─── Extension methods (additional) ──────────────────────────────────────

    [Fact]
    public void ReportCancelled_SetsCorrectTypeAndDefaultMessage() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        // act
        worker.ReportCancelled();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(WorkerProgressType.Cancelled, worker.LastProgress?.Type);
            Assert.Contains(worker.Name, worker.LastProgress?.Message);
        });

        worker.Dispose();
    }

    [Fact]
    public void ReportCancelled_WithCustomMessage_UsesCustomMessage() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        // act
        worker.ReportCancelled("custom cancel message");

        // assert
        Assert.Equal("custom cancel message", worker.LastProgress?.Message);

        worker.Dispose();
    }

    [Fact]
    public void ReportInformation_WithMetadataAndPercentage_SetsAllProperties() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );
        var metadata = new Dictionary<string, object> { ["key"] = "value" };

        // act
        worker.ReportInformation("info message", percentageComplete: 75, metadata: metadata);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(WorkerProgressType.Information, worker.LastProgress?.Type);
            Assert.Equal("info message", worker.LastProgress?.Message);
            Assert.Equal(75, worker.LastProgress?.PercentageComplete);
            Assert.NotNull(worker.LastProgress?.Metadata);
            Assert.Equal("value", worker.LastProgress!.Metadata!["key"]);
        });

        worker.Dispose();
    }

    [Fact]
    public void ReportTickFailed_WithNullException_ThrowsArgumentNullException() {
        // arrange
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        // act & assert
        Assert.Throws<ArgumentNullException>(() => worker.ReportTickFailed(null!));

        worker.Dispose();
    }

    // ─── Subscribe lifecycle ──────────────────────────────────────────────────

    [Fact]
    public async Task Progress_SubscribeAfterStopAsync_ImmediatelyCallsOnCompleted() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var completedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        await worker.StartAsync(ct);
        await worker.StopAsync(ct);

        // act
        worker.Progress.Subscribe(new CompletionObserver(completedTcs));

        // assert
        Assert.True(completedTcs.Task.IsCompletedSuccessfully);

        worker.Dispose();
    }

    [Fact]
    public void Progress_SubscribeAfterDispose_ImmediatelyCallsOnCompleted() {
        // arrange
        var completedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker)),
            NullLogger<Worker>.Instance
        );

        worker.Dispose();

        // act
        worker.Progress.Subscribe(new CompletionObserver(completedTcs));

        // assert
        Assert.True(completedTcs.Task.IsCompletedSuccessfully);
    }

    // ─── NullWorkerObservable ─────────────────────────────────────────────

    [Fact]
    public void NullWorkerObservable_Subscribe_ImmediatelyCallsOnCompleted() {
        // arrange
        var completedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var observer = new CompletionObserver(completedTcs);

        // act
        NullWorkerObservable.Instance.Subscribe(observer);

        // assert
        Assert.True(completedTcs.Task.IsCompletedSuccessfully);
    }

    [Fact]
    public void NullWorkerObservable_Instance_ReturnsSameSingleton() {
        Assert.Same(NullWorkerObservable.Instance, NullWorkerObservable.Instance);
    }

    // ─── test doubles ─────────────────────────────────────────────────────

    private sealed class SimpleWorker(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger<Worker> logger)
        : Worker(configuration, logger) {

        public override string Name => nameof(SimpleWorker);

        public override Task DoWorkAsync(CancellationToken cancellationToken)
            => Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private sealed class ControllableWorker(
        IConfiguration configuration,
        Microsoft.Extensions.Logging.ILogger<Worker> logger,
        Func<Worker, Task> work)
        : Worker(configuration, logger) {

        public override string Name => nameof(ControllableWorker);

        public override Task DoWorkAsync(CancellationToken cancellationToken) => work(this);
    }

    private sealed class CollectingObserver(List<WorkerProgress> target) : IObserver<WorkerProgress> {
        public void OnNext(WorkerProgress value) => target.Add(value);
        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }

    private sealed class ThrowingObserver : IObserver<WorkerProgress> {
        public void OnNext(WorkerProgress value) => throw new InvalidOperationException("observer fault");
        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }

    private sealed class CompletionObserver(TaskCompletionSource tcs) : IObserver<WorkerProgress> {
        public void OnNext(WorkerProgress value) { }
        public void OnError(Exception error) { }
        public void OnCompleted() => tcs.TrySetResult();
    }
}
