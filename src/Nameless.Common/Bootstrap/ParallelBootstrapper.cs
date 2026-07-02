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

    private readonly IOptions<BootstrapOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParallelBootstrapper"/> class.
    /// </summary>
    /// <param name="retryPipelineFactory">The retry policy factory.</param>
    /// <param name="steps">The collection of steps to execute.</param>
    /// <param name="timeProvider">The time provider.</param>
    /// <param name="options">The bootstrap options (used to configure parallelism).</param>
    /// <param name="logger">The logger.</param>
    public ParallelBootstrapper(
        IRetryPipelineFactory retryPipelineFactory,
        IEnumerable<IStep> steps,
        TimeProvider timeProvider,
        IOptions<BootstrapOptions> options,
        ILogger<ParallelBootstrapper> logger)
        : base(retryPipelineFactory, steps, timeProvider, logger) {
        _options = options;
    }

    /// <inheritdoc />
    protected override async Task ExecuteStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionGraph graph, CancellationToken cancellationToken) {
        foreach (var level in graph) {
            RunStepExecutionLevelAsync handler = level.Count switch {
                1 => ExecuteSingleStepAsync,
                _ => ExecuteMultipleStepsAsync,
            };

            await handler(context, progress, level, cancellationToken).SkipContextSync();
        }
    }

    private async Task ExecuteSingleStepAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken) {
        await ExecuteStepWithRetryAsync(
            context,
            node: level.Single(),
            progress,
            cancellationToken
        ).SkipContextSync();
    }

    private async Task ExecuteMultipleStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken) {
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
