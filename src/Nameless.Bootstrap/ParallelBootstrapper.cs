using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;
using Nameless.Bootstrap.Resilience;

namespace Nameless.Bootstrap;

public class ParallelBootstrapper : Bootstrapper {
    private delegate Task RunStepExecutionLevelAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken);

    private readonly ILogger<ParallelBootstrapper> _logger;
    private readonly IOptions<BootstrapOptions> _options;

    public ParallelBootstrapper(
        IEnumerable<IStep> steps,
        IRetryPolicyFactory retryPolicyFactory,
        TimeProvider timeProvider,
        ILogger<ParallelBootstrapper> logger,
        IOptions<BootstrapOptions> options)
        : base(steps, retryPolicyFactory, timeProvider, logger) {
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionGraph graph, CancellationToken cancellationToken) {
        _logger.ExecutingStepsInParallel();

        foreach (var level in graph) {
            _logger.ExecutingStepExecutionLevel(level.Level, graph.Count, level.Count);

            RunStepExecutionLevelAsync handler = level.Count switch {
                1 => ExecuteSingleStepAsync,
                _ => ExecuteMultipleStepsAsync,
            };

            await handler(context, progress, level, cancellationToken).SkipContextSync();
        }
    }

    private async Task ExecuteSingleStepAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken) {
        var node = level.Single();

        _logger.ExecutingOneStepInSequence(node.Step.Name);

        await ExecuteStepWithRetryAsync(
            context,
            node,
            progress,
            cancellationToken
        ).SkipContextSync();
    }

    private async Task ExecuteMultipleStepsAsync(FlowContext context, IProgress<StepProgress> progress, StepExecutionLevel level, CancellationToken cancellationToken) {
        _logger.ExecutingMultipleStepsInParallel([.. level.Select(node => node.Step.Name)]);

        var parallelOptions = new ParallelOptions {
            MaxDegreeOfParallelism = _options.Value.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(
            level,
            parallelOptions,
            async (node, token) => await ExecuteStepWithRetryAsync(
                context,
                node,
                progress,
                token
            ).SkipContextSync()
        ).SkipContextSync();
    }
}
