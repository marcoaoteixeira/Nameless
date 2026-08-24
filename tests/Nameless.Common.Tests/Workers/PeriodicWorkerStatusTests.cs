using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Workers;

public class PeriodicWorkerStatusTests {
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
        var worker = new NeverExecutingPeriodicWorker(
            CreateConfiguration(nameof(NeverExecutingPeriodicWorker)),
            NullLogger<PeriodicWorker>.Instance
        );

        // act
        await worker.StartAsync(ct);

        // assert
        Assert.Equal(PeriodicWorkerStatus.Idle, worker.Status);

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsRunning_WhileDoWorkAsyncExecutes() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var enterTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllablePeriodicWorker(
            CreateConfiguration(nameof(ControllablePeriodicWorker)),
            NullLogger<PeriodicWorker>.Instance,
            async innerCt => {
                enterTcs.TrySetResult();
                await releaseTcs.Task.WaitAsync(innerCt);
            }
        );

        await worker.StartAsync(ct);

        // wait for DoWorkAsync to enter
        await enterTcs.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

        // act / assert
        Assert.Equal(PeriodicWorkerStatus.Running, worker.Status);

        releaseTcs.TrySetResult();
        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsIdle_AfterDoWorkAsyncReturns() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var executedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllablePeriodicWorker(
            CreateConfiguration(nameof(ControllablePeriodicWorker)),
            NullLogger<PeriodicWorker>.Instance,
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
        Assert.Equal(PeriodicWorkerStatus.Idle, worker.Status);

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsFaulted_WhenDoWorkAsyncThrows() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var worker = new ControllablePeriodicWorker(
            CreateConfiguration(nameof(ControllablePeriodicWorker)),
            NullLogger<PeriodicWorker>.Instance,
            _ => throw new InvalidOperationException("test fault")
        );

        // ExecuteAsync re-throws, which surfaces through the hosted service task
        await worker.StartAsync(ct);

        // wait for the background task to fault
        await Task.Delay(300, ct);

        // act / assert
        Assert.Equal(PeriodicWorkerStatus.Faulted, worker.Status);

        worker.Dispose();
    }

    [Fact]
    public async Task Status_IsStopped_WhenCancelled() {
        // arrange
        var cts = new CancellationTokenSource();
        var worker = new NeverExecutingPeriodicWorker(
            CreateConfiguration(nameof(NeverExecutingPeriodicWorker)),
            NullLogger<PeriodicWorker>.Instance
        );

        await worker.StartAsync(cts.Token);

        // act
        await cts.CancelAsync();
        await worker.StopAsync(cts.Token);

        // assert
        Assert.Equal(PeriodicWorkerStatus.Stopped, worker.Status);

        worker.Dispose();
    }

    [Fact]
    public async Task Status_CanBeReadFromDifferentThread_WithoutDeadlock() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var enterTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var worker = new ControllablePeriodicWorker(
            CreateConfiguration(nameof(ControllablePeriodicWorker)),
            NullLogger<PeriodicWorker>.Instance,
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
        Assert.Equal(PeriodicWorkerStatus.Running, statusFromOtherThread);

        releaseTcs.TrySetResult();
        await worker.StopAsync(ct);
        worker.Dispose();
    }

    // ─── test doubles ─────────────────────────────────────────────────────

    private sealed class NeverExecutingPeriodicWorker(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger<PeriodicWorker> logger)
        : PeriodicWorker(configuration, logger) {

        public override string Name => nameof(NeverExecutingPeriodicWorker);

        public override Task DoWorkAsync(CancellationToken cancellationToken) {
            return Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }

    private sealed class ControllablePeriodicWorker(
        IConfiguration configuration,
        Microsoft.Extensions.Logging.ILogger<PeriodicWorker> logger,
        Func<CancellationToken, Task> work)
        : PeriodicWorker(configuration, logger) {

        public override string Name => nameof(ControllablePeriodicWorker);

        public override Task DoWorkAsync(CancellationToken cancellationToken) {
            return work(cancellationToken);
        }
    }
}
