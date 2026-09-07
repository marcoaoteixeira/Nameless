using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Configuration;
using Nameless.Reporting;
using Nameless.Testing.Tools.Mockers.StatusReporting;

namespace Nameless.Workers;

public class WorkerConfigTests {
    // ─── IsDisabled ──────────────────────────────────────────────────────────

    [Fact]
    public void IsDisabled_WhenIsEnabledFalse_ReturnsTrue() {
        // arrange
        var options = new PeriodicWorkerOptions { IsEnabled = false, Interval = TimeSpan.FromMilliseconds(50) };

        // act & assert
        Assert.True(options.IsDisabled);
    }

    [Fact]
    public void IsDisabled_WhenIsEnabledTrue_ReturnsFalse() {
        // arrange
        var options = new PeriodicWorkerOptions { IsEnabled = true, Interval = TimeSpan.FromMilliseconds(50) };

        // act & assert
        Assert.False(options.IsDisabled);
    }

    // ─── Disabled worker does not call DoWorkAsync ────────────────────────────

    [Fact]
    public async Task StartAsync_WithIsEnabledFalse_DoWorkAsyncNeverCalled() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var workCalled = false;

        var worker = new ControllablePeriodicWorker(
            CreateConfiguration(nameof(ControllablePeriodicWorker), isEnabled: false),
            NullLogger<PeriodicWorker>.Instance,
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

        var worker = new SimplePeriodicWorker(
            configuration,
            new StatusReporterMocker<SimplePeriodicWorker>().Build(),
            NullLogger<PeriodicWorker>.Instance
        );

        // act
        await worker.StartAsync(ct);

        // give background task time to fault
        await Task.Delay(200, ct);

        // assert: the background ExecuteTask is faulted with InvalidOperationException
        Assert.NotNull(worker.ExecuteTask);
        Assert.True(worker.ExecuteTask.IsFaulted);
        Assert.IsType<MissingConfigurationException>(
            worker.ExecuteTask.Exception!.InnerExceptions[0]
        );

        worker.Dispose();
    }

    // ─── Zero interval ───────────────────────────────────────────────────────

    [Fact]
    public async Task StartAsync_WithZeroInterval_BackgroundTaskFaultsWithInvalidOperationException() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var worker = new SimplePeriodicWorker(
            CreateConfiguration(nameof(SimplePeriodicWorker), isEnabled: true, interval: "00:00:00"),
            new StatusReporterMocker<SimplePeriodicWorker>().Build(),
            NullLogger<PeriodicWorker>.Instance
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

    private sealed class SimplePeriodicWorker(
        IConfiguration configuration,
        IStatusReporter<SimplePeriodicWorker> statusReporter,
        ILogger<PeriodicWorker> logger)
        : PeriodicWorker(configuration, statusReporter, logger) {
        public override string Name => nameof(SimplePeriodicWorker);
        public override Task DoWorkAsync(CancellationToken ct) {
            return Task.Delay(Timeout.Infinite, ct);
        }
    }

    private sealed class ControllablePeriodicWorker(IConfiguration configuration, ILogger<PeriodicWorker> logger, Func<CancellationToken, Task> work)
        : PeriodicWorker(configuration, new StatusReporterMocker<ControllablePeriodicWorker>().Build(), logger) {
        public override string Name => nameof(ControllablePeriodicWorker);
        public override Task DoWorkAsync(CancellationToken ct) {
            return work(ct);
        }
    }
}
