using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Notification;
using Nameless.Resilience;

namespace Nameless.Bootstrap;

public class Bootstrapper : IBootstrapper {
    private readonly IStep[] _steps;
    private readonly IRetryPipelineFactory _retryPipelineFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<Bootstrapper> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bootstrapper"/> class.
    /// </summary>
    /// <param name="steps">
    ///     The collection of steps to be executed during the bootstrap
    ///     process.
    /// </param>
    /// <param name="retryPipelineFactory">
    ///     The retry policy factory.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="logger">
    ///     The logger used to record execution details and diagnostic
    ///     information.
    /// </param>
    public Bootstrapper(IEnumerable<IStep> steps, IRetryPipelineFactory retryPipelineFactory, TimeProvider timeProvider, ILogger<Bootstrapper> logger) {
        _steps = [.. steps];
        _timeProvider = timeProvider;
        _retryPipelineFactory = retryPipelineFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        _logger.BootstrapStarting(_steps.Length);

        var sw = Stopwatch.StartNew();
        var graph = StepExecutionGraphBuilder.Create(_steps);

        _logger.StepDependencyGraphBuilt(graph.LevelCount, graph.TotalSteps);

        try {
            await ExecuteStepsAsync(
                context,
                progress,
                graph,
                cancellationToken
            ).SkipContextSync();
        }
        catch (Exception ex) {
            _logger.Failure(ex);

            throw;
        }

        var results = graph.GetExecutionResults().ToArray();

        _logger.BootstrapFinished(
            sw.ElapsedMilliseconds,
            results.Count(result => result.Success),
            results.Length
        );

        _logger.WriteExecutionStatistics(results);

        if (results.Any(result => !result.Success)) {
            throw new BootstrapException("One or more steps failed.", results);
        }
    }

    protected virtual async Task ExecuteStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionGraph graph, CancellationToken cancellationToken) {
        _logger.ExecutionMode("SEQUENTIAL");

        var currentStep = 0;
        var totalSteps = _steps.Length;

        foreach (var level in graph) {
            foreach (var node in level) {
                _logger.CurrentlyExecutingStep(
                    ++currentStep,
                    totalSteps,
                    node.Step.DisplayName
                );

                await ExecuteStepWithRetryAsync(
                    context,
                    node,
                    progress,
                    cancellationToken
                ).SkipContextSync();
            }
        }
    }

    protected async Task ExecuteStepWithRetryAsync(FlowContext context, StepExecutionNode node, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        _logger.StepStarting(node.Step.DisplayName);

        var sw = Stopwatch.StartNew();

        node.Result.StartTime = _timeProvider.GetUtcNow();

        try {
            progress.ReportStart(node.Step.DisplayName);

            if (node.Step.IsDisabled) {
                _logger.StepDisabled(node.Step.DisplayName);

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

            _logger.StepFailure(node.Step.DisplayName, ex);
        }
        finally {
            node.Result.Duration = sw.Elapsed;

            _logger.StepFinished(node.Step.DisplayName, sw.ElapsedMilliseconds);
        }
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
