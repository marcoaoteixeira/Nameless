using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Workers;

namespace Nameless;

public class WorkerStatusTests {
    private static IConfiguration CreateConfiguration(string workerName, bool isEnabled = true, string interval = "00:00:00.050") {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                [$"Workers:{workerName}:IsEnabled"] = isEnabled ? "true" : "false",
                [$"Workers:{workerName}:Interval"] = interval,
            })
            .Build();
    }

    [Fact]
    public async Task Status_IsIdle_BeforeFirstTick() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var worker = new NeverExecutingWorker(
            CreateConfiguration(nameof(NeverExecutingWorker)),
            NullLogger<Worker>.Instance
        );

        // act
        await worker.StartAsync(ct);

        // assert
        Assert.Equal(WorkerStatus.Idle, worker.Status);

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsRunning_WhileDoWorkAsyncExecutes() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var enterTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            async innerCt => {
                enterTcs.TrySetResult();
                await releaseTcs.Task.WaitAsync(innerCt);
            }
        );

        await worker.StartAsync(ct);

        // wait for DoWorkAsync to enter
        await enterTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // act / assert
        Assert.Equal(WorkerStatus.Running, worker.Status);

        releaseTcs.TrySetResult();
        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsIdle_AfterDoWorkAsyncReturns() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var executedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            _ => {
                executedTcs.TrySetResult();
                return Task.CompletedTask;
            }
        );

        await worker.StartAsync(ct);
        await executedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // give ExecuteAsync a moment to set Idle after DoWorkAsync returns
        await Task.Delay(50, ct);

        // act / assert
        Assert.Equal(WorkerStatus.Idle, worker.Status);

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsFaulted_WhenDoWorkAsyncThrows() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            _ => throw new InvalidOperationException("test fault")
        );

        // ExecuteAsync re-throws, which surfaces through the hosted service task
        await worker.StartAsync(ct);

        // wait for the background task to fault
        await Task.Delay(300, ct);

        // act / assert
        Assert.Equal(WorkerStatus.Faulted, worker.Status);

        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsStopped_WhenCancelled() {
        // arrange
        var cts = new CancellationTokenSource();
        var worker = new NeverExecutingWorker(
            CreateConfiguration(nameof(NeverExecutingWorker)),
            NullLogger<Worker>.Instance
        );

        await worker.StartAsync(cts.Token);

        // act
        await cts.CancelAsync();
        await worker.StopAsync(cts.Token);

        // assert
        Assert.Equal(WorkerStatus.Stopped, worker.Status);

        worker.Dispose();
    }

    [Fact]
    public async Task Status_CanBeReadFromDifferentThread_WithoutDeadlock() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var enterTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker)),
            NullLogger<Worker>.Instance,
            async innerCt => {
                enterTcs.TrySetResult();
                await releaseTcs.Task.WaitAsync(innerCt);
            }
        );

        await worker.StartAsync(ct);
        await enterTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // act: read status from a separate thread
        var statusFromOtherThread = await Task.Run(() => worker.Status, ct);

        // assert
        Assert.Equal(WorkerStatus.Running, statusFromOtherThread);

        releaseTcs.TrySetResult();
        await worker.StopAsync(ct);
        worker.Dispose();
    }

    // ─── test doubles ─────────────────────────────────────────────────────

    private sealed class NeverExecutingWorker(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger<Worker> logger)
        : Worker(configuration, logger) {

        public override string Name => nameof(NeverExecutingWorker);

        public override Task DoWorkAsync(CancellationToken cancellationToken) {
            return Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }

    private sealed class ControllableWorker(
        IConfiguration configuration,
        Microsoft.Extensions.Logging.ILogger<Worker> logger,
        Func<CancellationToken, Task> work)
        : Worker(configuration, logger) {

        public override string Name => nameof(ControllableWorker);

        public override Task DoWorkAsync(CancellationToken cancellationToken) {
            return work(cancellationToken);
        }
    }
}
