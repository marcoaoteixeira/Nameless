using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;
using Nameless.Bootstrap.Resilience;
using Polly;

namespace Nameless.Bootstrap;

public class Bootstrapper : IBootstrapper {
    private readonly IEnumerable<IStep> _steps;
    private readonly IRetryPolicyFactory _retryPolicyFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<Bootstrapper> _logger;
    private readonly Lazy<IStep[]> _availableSteps;

    /// <summary>
    ///     Gets the available steps.
    /// </summary>
    protected IStep[] AvailableSteps => _availableSteps.Value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bootstrapper"/> class.
    /// </summary>
    /// <param name="steps">
    ///     The collection of steps to be executed during the bootstrap
    ///     process.
    /// </param>
    /// <param name="retryPolicyFactory">
    ///     The retry policy factory.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="logger">
    ///     The logger used to record execution details and diagnostic
    ///     information.
    /// </param>
    public Bootstrapper(IEnumerable<IStep> steps, IRetryPolicyFactory retryPolicyFactory, TimeProvider timeProvider, ILogger<Bootstrapper> logger) {
        _steps = steps;
        _timeProvider = timeProvider;
        _retryPolicyFactory = retryPolicyFactory;
        _logger = logger;

        _availableSteps = new Lazy<IStep[]>(GetAvailableSteps);
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        _logger.Starting(AvailableSteps.Length);

        var sw = Stopwatch.StartNew();
        var graph = StepExecutionGraphBuilder.Create(AvailableSteps);

        _logger.StepDependencyGraphBuilt(graph.Count, graph.TotalSteps);
        
        try {
            await ExecuteStepsAsync(
                context,
                progress,
                graph,
                cancellationToken
            ).SkipContextSync();
        }
        catch (Exception ex) { _logger.Failure(ex); }

        var results = graph.GetExecutionResults().ToArray();

        _logger.Finished(
            sw.ElapsedMilliseconds,
            results.Count(result => result.Success),
            results.Length
        );

        _logger.WriteExecutionStatistics(results);

        if (results.Any(result => !result.Success)) {
            throw new BootstrapException("One or more steps failed.");
        }
    }

    protected virtual async Task ExecuteStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionGraph graph, CancellationToken cancellationToken) {
        _logger.ExecutingStepsSequentially();

        var currentStep = 0;
        var totalSteps = AvailableSteps.Length;

        foreach (var level in graph) {
            foreach (var node in level) {
                _logger.ExecutingStepsStarting(++currentStep, totalSteps, node.Step.Name);

                await ExecuteStepWithRetryAsync(
                    context,
                    node,
                    progress,
                    cancellationToken
                ).SkipContextSync();
            }
        }
    }

    protected virtual async Task ExecuteStepWithRetryAsync(FlowContext context, StepExecutionNode node, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        _logger.StepStarting(node.Step.Name);

        var sw = Stopwatch.StartNew();

        node.Result.StartTime = _timeProvider.GetUtcNow();
        
        try {
            progress.ReportStart(node.Step.Name);

            var resiliencePipeline = CreateResiliencePipeline(node.Step, progress);

            await resiliencePipeline.ExecuteAsync(
                async token => await node.Step
                                         .ExecuteAsync(context, progress, token)
                                         .SkipContextSync(),
                cancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);

            progress.ReportComplete(node.Step.Name);
        }
        catch (Exception ex) {
            node.Result.Exception = ex;

            progress.ReportFailure(node.Step.Name, ex.Message, ex);

            _logger.StepFailure(node.Step.Name, ex);
        }
        finally {
            node.Result.Duration = sw.Elapsed;

            _logger.StepFinished(node.Step.Name, sw.ElapsedMilliseconds);
        }
    }

    private ResiliencePipeline CreateResiliencePipeline(IStep step, IProgress<StepProgress> progress) {
        if (step.RetryPolicy is null) { return ResiliencePipeline.Empty; }

        // Add callback to report retry via progress
        var policyWithProgress = step.RetryPolicy with {
            OnRetry = (ex, delay, attempt, maxAttempts) => {
                // invoke original callback
                step.RetryPolicy.OnRetry.Invoke(ex, delay, attempt, maxAttempts);

                // Report retry via progress
                progress.ReportRetrying(step.Name, attempt, maxAttempts, delay);
            }
        };

        return _retryPolicyFactory.CreateRetryPipeline(step.Name, policyWithProgress);
    }

    private IStep[] GetAvailableSteps() {
        IStep[] result = [.. _steps.Where(step => step.IsEnabled)];

        if (result.Length == 0) {
            _logger.UnavailableSteps();
        }

        return result;
    }
}
