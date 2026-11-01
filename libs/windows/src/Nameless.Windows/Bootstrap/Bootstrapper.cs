using Microsoft.Extensions.Logging;
using Nameless.Resilience;
using Nameless.Windows.Bootstrap.Execution;
using Nameless.Windows.Bootstrap.Notification;
using System.Diagnostics;
using Nameless.Windows.Localization;

namespace Nameless.Windows.Bootstrap;

/// <summary>
///     Default implementation of <see cref="IBootstrapper"/> that executes
///     bootstrap steps sequentially.
/// </summary>
public class Bootstrapper : IBootstrapper {
    private readonly IStep[] _steps;
    private readonly IRetryPipelineFactory _retryPipelineFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ITranslator _translator;
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
    /// <param name="translator">
    ///     The translator.
    /// </param>
    /// <param name="logger">
    ///     The logger used to record execution details and diagnostic
    ///     information.
    /// </param>
    public Bootstrapper(IEnumerable<IStep> steps, IRetryPipelineFactory retryPipelineFactory, TimeProvider timeProvider, ITranslator translator, ILogger<Bootstrapper> logger) {
        _steps = [.. steps];
        _retryPipelineFactory = retryPipelineFactory;
        _timeProvider = timeProvider;
        _translator = translator;
        _logger = logger;
    }

    /// <inheritdoc />
    /// <exception cref="BootstrapException">
    ///     if one or more steps fail during execution.
    /// </exception>
    public async Task RunAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportStart("Bootstrapper", _translator["Bootstrapper_RunAsync_Start"]);
        
        var graph = StepExecutionGraphBuilder.Create(_steps);
        progress.ReportInformation("Bootstrapper", _translator["Bootstrapper_RunAsync_DependencyGraphBuilt"]);

        await ExecuteStepsAsync(progress, graph, cancellationToken).SkipContextSync();

        var failures = graph.GetExecutionResults()
                            .Where(result => !result.Success)
                            .ToArray();

        if (failures.Length > 0) {
            progress.ReportFailure("Bootstrapper", _translator["Bootstrapper_RunAsync_Failure"]);

            throw new BootstrapException("One or more steps failed.", failures);
        }

        progress.ReportComplete("Bootstrapper", _translator["Bootstrapper_RunAsync_Finish"]);
    }

    /// <summary>
    ///     Executes all steps in the graph. Override this method to provide
    ///     a custom execution strategy, such as parallel execution.
    /// </summary>
    /// <param name="progress">The progress notifier.</param>
    /// <param name="graph">The execution graph that defines the order and dependencies of steps.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual async Task ExecuteStepsAsync(IProgress<StepProgress> progress, StepExecutionGraph graph, CancellationToken cancellationToken) {
        foreach (var level in graph) {
            foreach (var node in level) {
                await ExecuteStepWithRetryAsync(progress, node, cancellationToken).SkipContextSync();
            }
        }
    }

    /// <summary>
    ///     Executes a single step with retry support.
    /// </summary>
    /// <param name="progress">The progress notifier</param>
    /// <param name="node">The execution node representing the step to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task ExecuteStepWithRetryAsync(IProgress<StepProgress> progress, StepExecutionNode node, CancellationToken cancellationToken) {
        var sw = Stopwatch.StartNew();

        node.Result.StartTime = _timeProvider.GetUtcNow();

        try {
            if (node.Step.IsDisabled) { return; }

            progress.ReportStart(node.Step.Name, _translator["Bootstrapper_ExecuteStepWithRetryAsync_StepStart", node.Step.Name]);

            var retryPipeline = CreateRetryPipeline(node.Step);

            await retryPipeline.ExecuteAsync(
                async token => await node.Step
                                         .ExecuteAsync(progress, token)
                                         .SkipContextSync(),
                cancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);

            progress.ReportComplete(node.Step.Name, _translator["Bootstrapper_ExecuteStepWithRetryAsync_StepFinish", node.Step.Name]);
        }
        catch (Exception ex) {
            node.Result.Exception = ex;

            progress.ReportFailure(node.Step.Name, _translator["Bootstrapper_ExecuteStepWithRetryAsync_StepFailure", node.Step.Name, ex.Message], ex);

            CommonLog.Error(_logger, ex.Message, ex, tag: node.Step.DisplayName);

            throw;
        }
        finally { node.Result.Duration = sw.Elapsed; }
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
}
