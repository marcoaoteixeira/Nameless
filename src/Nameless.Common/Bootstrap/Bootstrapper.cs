using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Notification;
using Nameless.Resilience;

namespace Nameless.Bootstrap;

/// <summary>
///     Default implementation of <see cref="IBootstrapper"/> that executes
///     bootstrap steps sequentially.
/// </summary>
public class Bootstrapper : IBootstrapper {
    private readonly IStep[] _steps;
    private readonly IRetryPipelineFactory _retryPipelineFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<Bootstrapper> _logger;

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
    /// <param name="logger">
    ///     The logger used to record execution details and diagnostic
    ///     information.
    /// </param>
    public Bootstrapper(IRetryPipelineFactory retryPipelineFactory, IEnumerable<IStep> steps, TimeProvider timeProvider, ILogger<Bootstrapper> logger) {
        _retryPipelineFactory = retryPipelineFactory;
        _steps = [.. steps];
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    /// <exception cref="BootstrapException">
    ///     if one or more steps fail during execution.
    /// </exception>
    public async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        var graph = StepExecutionGraphBuilder.Create(_steps);

        await ExecuteStepsAsync(context, progress, graph, cancellationToken).SkipContextSync();

        var results = graph.GetExecutionResults().ToArray();

        if (results.Any(result => !result.Success)) {
            throw new BootstrapException("One or more steps failed.", results);
        }
    }

    /// <summary>
    ///     Executes all steps in the graph. Override this method to provide
    ///     a custom execution strategy, such as parallel execution.
    /// </summary>
    /// <param name="context">The flow context shared across steps.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="graph">The execution graph that defines the order and dependencies of steps.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionGraph graph, CancellationToken cancellationToken) {
        foreach (var level in graph) {
            foreach (var node in level) {
                await ExecuteStepWithRetryAsync(
                    context,
                    node,
                    progress,
                    cancellationToken
                ).SkipContextSync();
            }
        }
    }

    /// <summary>
    ///     Executes a single step with retry support.
    /// </summary>
    /// <param name="context">The flow context shared across steps.</param>
    /// <param name="node">The execution node representing the step to run.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task ExecuteStepWithRetryAsync(FlowContext context, StepExecutionNode node, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();

        node.Result.StartTime = _timeProvider.GetUtcNow();

        try {
            progress.ReportStart(node.Step.DisplayName);

            if (node.Step.IsDisabled) {
                progress.ReportComplete(node.Step.DisplayName);

                return;
            }

            var retryPipeline = CreateRetryPipeline(node.Step, progress);

            await retryPipeline.ExecuteAsync(
                async token => await node.Step
                                         .ExecuteAsync(context, progress, token)
                                         .SkipContextSync(),
                cancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);

            progress.ReportComplete(node.Step.DisplayName);
        }
        catch (Exception ex) {
            node.Result.Exception = ex;

            progress.ReportFailure(node.Step.DisplayName, ex.Message, ex);

            Log.ExecuteStepWithRetryAsyncFailure(
                _logger,
                node.Step.DisplayName,
                ex
            );
        }
        finally { node.Result.Duration = sw.Elapsed; }
    }

    private IRetryPipeline CreateRetryPipeline(IStep step, IProgress<StepProgress> progress) {
        if (step.RetryPolicy is null) { return RetryPipeline.Empty; }

        // Add callback to report retry via progress
        var configuration = step.RetryPolicy with {
            Tag = step.RetryPolicy.Tag ?? step.DisplayName,

            OnRetry = (ex, delay, attempt, maxAttempts) => {
                // invoke original callback
                step.RetryPolicy.OnRetry.Invoke(ex, delay, attempt, maxAttempts);

                // Report retry via progress
                progress.ReportRetrying(step.DisplayName, attempt, maxAttempts, delay);
            }
        };

        return _retryPipelineFactory.Create(configuration);
    }
}
