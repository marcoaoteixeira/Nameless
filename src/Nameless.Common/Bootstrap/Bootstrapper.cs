using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Notification;
using Nameless.Reporting;
using Nameless.Resilience;

namespace Nameless.Bootstrap;

/// <summary>
///     Default implementation of <see cref="IBootstrapper"/> that executes
///     bootstrap steps sequentially.
/// </summary>
[StatusReporting]
public class Bootstrapper : IBootstrapper, IDisposable {
    private readonly IStep[] _steps;
    private readonly IRetryPipelineFactory _retryPipelineFactory;
    private readonly TimeProvider _timeProvider;
    private readonly IStatusReporter<Bootstrapper> _statusReporter;
    private readonly ILogger<Bootstrapper> _logger;
    private readonly IProgress<StepProgress> _progress;

    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bootstrapper"/> class.
    /// </summary>
    /// <param name="retryPipelineFactory">
    ///     The retry policy factory.
    /// </param>
    /// <param name="steps">
    ///     The collection of steps to be executed during the bootstrap
    ///     process.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="statusReporter">
    ///     The status reporter.
    /// </param>
    /// <param name="logger">
    ///     The logger used to record execution details and diagnostic
    ///     information.
    /// </param>
    public Bootstrapper(IEnumerable<IStep> steps, IRetryPipelineFactory retryPipelineFactory, TimeProvider timeProvider, IStatusReporter<Bootstrapper> statusReporter, ILogger<Bootstrapper> logger) {
        _steps = [.. steps];
        _retryPipelineFactory = retryPipelineFactory;
        _timeProvider = timeProvider;
        _statusReporter = statusReporter;
        _logger = logger;
        _progress = new Progress<StepProgress>(HandleStepProgress);
    }

    /// <summary>
    ///     Destructor.
    /// </summary>
    ~Bootstrapper() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    /// <exception cref="BootstrapException">
    ///     if one or more steps fail during execution.
    /// </exception>
    public async Task RunAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        var graph = StepExecutionGraphBuilder.Create(_steps);

        _statusReporter.ReportStart(time: _timeProvider.GetUtcNow());

        await ExecuteStepsAsync(graph, cancellationToken).SkipContextSync();

        var failures = graph.GetExecutionResults()
                            .Where(result => !result.Success)
                            .ToArray();

        if (failures.Length > 0) {
            _statusReporter.Fault(time: _timeProvider.GetUtcNow(), failures);

            throw new BootstrapException("One or more steps failed.", failures);
        }

        _statusReporter.Complete(time: _timeProvider.GetUtcNow());
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Executes all steps in the graph. Override this method to provide
    ///     a custom execution strategy, such as parallel execution.
    /// </summary>
    /// <param name="graph">The execution graph that defines the order and dependencies of steps.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteStepsAsync(StepExecutionGraph graph, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        foreach (var level in graph) {
            foreach (var node in level) {
                await ExecuteStepWithRetryAsync(node, cancellationToken).SkipContextSync();
            }
        }
    }

    /// <summary>
    ///     Executes a single step with retry support.
    /// </summary>
    /// <param name="node">The execution node representing the step to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task ExecuteStepWithRetryAsync(StepExecutionNode node, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        var sw = Stopwatch.StartNew();

        node.Result.StartTime = _timeProvider.GetUtcNow();

        _statusReporter.ReportStepStarting(
            time: node.Result.StartTime,
            step: node.Step
        );

        try {
            if (node.Step.IsDisabled) { return; }

            var retryPipeline = CreateRetryPipeline(node.Step);

            using var meter = DiagnosticsHelper.CreateStopwatchHistogram(
                name: Metrics.StepDuration,
                description: node.Step.DisplayName
            );

            await retryPipeline.ExecuteAsync(
                async token => await node.Step
                                         .ExecuteAsync(_progress, token)
                                         .SkipContextSync(),
                cancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex) {
            node.Result.Exception = ex;

            _statusReporter.ReportStepFailure(
                time: _timeProvider.GetUtcNow(),
                step: node.Step,
                ex
            );

            CommonLog.Failure(_logger, ex, tag: node.Step.DisplayName);
        }
        finally {
            node.Result.Duration = sw.Elapsed;

            _statusReporter.ReportStepFinish(
                time: _timeProvider.GetUtcNow(),
                step: node.Step
            );
        }
    }

    /// <summary>
    ///     Disposes the Bootstrapper.
    /// </summary>
    /// <param name="disposing">
    ///     Whether it should dispose the managed resources.
    /// </param>
    protected virtual void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            if (_statusReporter is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private IRetryPipeline CreateRetryPipeline(IStep step) {
        if (step.RetryPolicy is null) { return RetryPipeline.Empty; }

        // Add callback to report retry via progress
        var configuration = step.RetryPolicy with {
            Tag = step.RetryPolicy.Tag ?? step.DisplayName,

            // invoke original callback
            OnRetry = step.RetryPolicy.OnRetry
        };

        return _retryPipelineFactory.Create(configuration);
    }

    private void HandleStepProgress(StepProgress progress) {
        _statusReporter.ReportStepProgress(
            time: _timeProvider.GetUtcNow(),
            progress
        );
    }

    internal static class Metrics {
        private const string ROOT = "nameless.bootstrapper";

        internal const string StepDuration = $"{ROOT}.step.duration";
    }
}
