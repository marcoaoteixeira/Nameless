using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics;

namespace Nameless.Bootstrap;

/// <summary>
///     Coordinates the execution of a sequence of startup steps for
///     application initialization.
/// </summary>
/// <remarks>
///     The Bootstrapper is responsible for running all configured steps in
///     order to initialize the application. It is typically used at
///     application startup to ensure that all required initialization logic is
///     executed before the application begins handling requests. This class is
///     not thread-safe and should be used only during the application's
///     startup phase.
/// </remarks>
public class BootstrapExecutor : IBootstrapExecutor {
    public static readonly string ActivitySourceName = typeof(BootstrapExecutor).FullName ?? nameof(BootstrapExecutor);

    private readonly IActivitySource _activitySource;
    private readonly IStep[] _steps;
    private readonly ILogger<BootstrapExecutor> _logger;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapExecutor"/> class.
    /// </summary>
    /// <param name="activitySourceProvider">
    ///     The provider used to create the activity source for tracing
    ///     execution steps.
    /// </param>
    /// <param name="steps">
    ///     The collection of steps to be executed during the bootstrap
    ///     process.
    /// </param>
    /// <param name="logger">
    ///     The logger used to record execution details and diagnostic
    ///     information.
    /// </param>
    public BootstrapExecutor(
        IActivitySourceProvider activitySourceProvider,
        IEnumerable<IStep> steps,
        ILogger<BootstrapExecutor> logger) {
        _activitySource = activitySourceProvider.Create(ActivitySourceName);
        _steps = [.. steps];
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken) {
        _logger.BootstrapperInitializing();

        using var bootstrapActivity = _activitySource.StartActivity(
            Metrics.BOOTSTRAP_EXECUTOR_ACTIVITY_NAME
        );

        var bootstrapSw = Stopwatch.StartNew();
        var flowContext = new FlowContext();
        var currentStep = 0;

        foreach (var step in _steps) {
            var stepSw = Stopwatch.StartNew();

            using var stepActivity = _activitySource.StartActivity(
                string.Format(Metrics.STEP_ACTIVITY_NAME_PATTERN, step.Name)
            );

            stepActivity.SetTag(Metrics.STEP_NAME, step.Name)
                        .SetTag(Metrics.STEP_INDEX, ++currentStep)
                        .SetTag(Metrics.STEP_TOTAL, _steps.Length);

            try {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.StepInitializing(step);

                await step.ExecuteAsync(flowContext, cancellationToken)
                          .SkipContextSync();

                _logger.StepSuccess(step);

                stepActivity.SetTag(Metrics.STEP_DURATION_MS, stepSw.ElapsedMilliseconds)
                            .SetStatus(ActivityStatusCode.Ok);
            }
            catch (OperationCanceledException) {
                AuditCancellation(stepActivity, stepSw);

                throw;
            }
            catch (Exception ex) {
                AuditException(step, ex, stepActivity, stepSw);

                throw new BootstrappingException(step.Name, ex);
            }
        }

        _logger.BootstrapperSuccess();

        bootstrapActivity.SetTag(Metrics.BOOTSTRAP_DURATION_MS, bootstrapSw.ElapsedMilliseconds)
                         .SetTag(Metrics.BOOTSTRAP_TOTAL_STEPS, currentStep)
                         .SetStatus(ActivityStatusCode.Ok);
    }

    private void AuditCancellation(IActivity activity, Stopwatch sw) {
        _logger.BootstrapperCancellation();

        activity.SetTag(Metrics.STEP_DURATION_MS, sw.ElapsedMilliseconds)
                .SetTag(Metrics.BOOTSTRAP_CANCELLED, true)
                .SetStatus(ActivityStatusCode.Ok);
    }

    private void AuditException(IStep step, Exception ex, IActivity activity, Stopwatch sw) {
        _logger.StepException(step, ex);

        activity.SetTag(Metrics.STEP_DURATION_MS, sw.ElapsedMilliseconds)
                .AddException(ex)
                .SetStatus(ActivityStatusCode.Error);
    }

    internal static class Metrics {
        internal const string BOOTSTRAP_EXECUTOR_ACTIVITY_NAME = $"{nameof(BootstrapExecutor)}.{nameof(ExecuteAsync)}";
        internal const string STEP_ACTIVITY_NAME_PATTERN = $"{nameof(BootstrapExecutor)}.{nameof(IStep)}:{{0}}";

        internal const string BOOTSTRAP_TOTAL_STEPS = "bootstrap.total_steps";
        internal const string BOOTSTRAP_DURATION_MS = "bootstrap.duration_ms";
        internal const string BOOTSTRAP_CANCELLED = "bootstrap.cancelled";

        internal const string STEP_NAME = "step.name";
        internal const string STEP_INDEX = "step.index";
        internal const string STEP_TOTAL = "step.total";
        internal const string STEP_DURATION_MS = "step.duration_ms";
    }
}