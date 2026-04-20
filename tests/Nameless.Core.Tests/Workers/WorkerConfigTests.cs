using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Workers;

namespace Nameless;

public class WorkerConfigTests {
    // ─── IsDisabled ──────────────────────────────────────────────────────────

    [Fact]
    public void IsDisabled_WhenIsEnabledFalse_ReturnsTrue() {
        // arrange
        var options = new WorkerOptions { IsEnabled = false, Interval = TimeSpan.FromMilliseconds(50) };

        // act & assert
        Assert.True(options.IsDisabled);
    }

    [Fact]
    public void IsDisabled_WhenIsEnabledTrue_ReturnsFalse() {
        // arrange
        var options = new WorkerOptions { IsEnabled = true, Interval = TimeSpan.FromMilliseconds(50) };

        // act & assert
        Assert.False(options.IsDisabled);
    }

    // ─── Disabled worker does not call DoWorkAsync ────────────────────────────

    [Fact]
    public async Task StartAsync_WithIsEnabledFalse_DoWorkAsyncNeverCalled() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var workCalled = false;

        var worker = new ControllableWorker(
            CreateConfiguration(nameof(ControllableWorker), isEnabled: false),
            NullLogger<Worker>.Instance,
            _ => {
                workCalled = true;
                return Task.CompletedTask;
            }
        );

        // act
        await worker.StartAsync(ct);
        await Task.Delay(200, ct);

        // assert
        Assert.False(workCalled);

        await worker.StopAsync(ct);
        worker.Dispose();
    }

    // ─── Missing configuration entry ─────────────────────────────────────────

    [Fact]
    public async Task StartAsync_WithMissingConfigEntry_BackgroundTaskFaultsWithInvalidOperationException() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var worker = new SimpleWorker(configuration, NullLogger<Worker>.Instance);

        // act
        await worker.StartAsync(ct);

        // give background task time to fault
        await Task.Delay(200, ct);

        // assert: the background ExecuteTask is faulted with InvalidOperationException
        Assert.NotNull(worker.ExecuteTask);
        Assert.True(worker.ExecuteTask.IsFaulted);
        Assert.IsType<InvalidOperationException>(
            worker.ExecuteTask.Exception!.InnerExceptions[0]
        );

        worker.Dispose();
    }

    // ─── Zero interval ───────────────────────────────────────────────────────

    [Fact]
    public async Task StartAsync_WithZeroInterval_BackgroundTaskFaultsWithInvalidOperationException() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var worker = new SimpleWorker(
            CreateConfiguration(nameof(SimpleWorker), isEnabled: true, interval: "00:00:00"),
            NullLogger<Worker>.Instance
        );

        // act
        await worker.StartAsync(ct);

        // give background task time to fault
        await Task.Delay(200, ct);

        // assert: the background ExecuteTask is faulted with InvalidOperationException
        Assert.NotNull(worker.ExecuteTask);
        Assert.True(worker.ExecuteTask.IsFaulted);
        Assert.IsType<InvalidOperationException>(
            worker.ExecuteTask.Exception!.InnerExceptions[0]
        );

        worker.Dispose();
    }

    // ─── helpers ─────────────────────────────────────────────────────────────

    private static IConfiguration CreateConfiguration(
        string workerName,
        bool isEnabled = true,
        string interval = "00:00:00.050") {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                [$"Workers:{workerName}:IsEnabled"] = isEnabled.ToString(),
                [$"Workers:{workerName}:Interval"] = interval,
            })
            .Build();
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class SimpleWorker(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger<Worker> logger)
        : Worker(configuration, logger) {
        public override string Name => nameof(SimpleWorker);
        public override Task DoWorkAsync(CancellationToken ct) => Task.Delay(Timeout.Infinite, ct);
    }

    private sealed class ControllableWorker(
        IConfiguration configuration,
        Microsoft.Extensions.Logging.ILogger<Worker> logger,
        Func<CancellationToken, Task> work)
        : Worker(configuration, logger) {
        public override string Name => nameof(ControllableWorker);
        public override Task DoWorkAsync(CancellationToken ct) => work(ct);
    }
}
