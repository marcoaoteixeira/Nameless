using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Notification;
using Nameless.Resilience;

namespace Nameless.Bootstrap;

/// <summary>
///     A <see cref="Bootstrapper"/> implementation that executes steps within
///     each dependency level in parallel.
/// </summary>
public class ParallelBootstrapper : Bootstrapper {
    private delegate Task RunStepExecutionLevelAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken);

    private readonly ILogger<ParallelBootstrapper> _logger;
    private readonly IOptions<BootstrapOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParallelBootstrapper"/> class.
    /// </summary>
    /// <param name="steps">The collection of steps to execute.</param>
    /// <param name="retryPipelineFactory">The retry policy factory.</param>
    /// <param name="timeProvider">The time provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The bootstrap options (used to configure parallelism).</param>
    public ParallelBootstrapper(
        IEnumerable<IStep> steps,
        IRetryPipelineFactory retryPipelineFactory,
        TimeProvider timeProvider,
        ILogger<ParallelBootstrapper> logger,
        IOptions<BootstrapOptions> options)
        : base(steps, retryPipelineFactory, timeProvider, logger) {
        _logger = logger;
        _options = options;
    }

    /// <inheritdoc />
    protected override async Task ExecuteStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionGraph graph, CancellationToken cancellationToken) {
        _logger.ExecutionMode("PARALLEL");

        foreach (var level in graph) {
            _logger.ExecutingStepInLevel(level.Level, graph.LevelCount, level.Count);

            RunStepExecutionLevelAsync handler = level.Count switch {
                1 => ExecuteSingleStepAsync,
                _ => ExecuteMultipleStepsAsync,
            };

            await handler(context, progress, level, cancellationToken).SkipContextSync();
        }
    }

    private async Task ExecuteSingleStepAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken) {
        var node = level.Single();

        _logger.ExecutingSingleStepInLevel(node.Step.DisplayName);

        await ExecuteStepWithRetryAsync(
            context,
            node,
            progress,
            cancellationToken
        ).SkipContextSync();
    }

    private async Task ExecuteMultipleStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken) {
        _logger.ExecutingMultipleStepInLevel([.. level.Select(node => node.Step.DisplayName)]);

        var parallelOptions = new ParallelOptions {
            MaxDegreeOfParallelism = _options.Value.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(
            source: level,
            parallelOptions,
            body: async (node, token) => await ExecuteStepWithRetryAsync(
                context,
                node,
                progress,
                token
            ).SkipContextSync()
        ).SkipContextSync();
    }
}
