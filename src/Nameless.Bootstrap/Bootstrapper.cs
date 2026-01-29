using System.Diagnostics;
using Microsoft.Extensions.Logging;

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
public class Bootstrapper : IBootstrapper {
    private readonly IStep[] _steps;
    private readonly ILogger<Bootstrapper> _logger;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="Bootstrapper"/> class.
    /// </summary>
    /// <param name="steps">
    ///     The collection of steps to be executed during the bootstrap
    ///     process.
    /// </param>
    /// <param name="logger">
    ///     The logger used to record execution details and diagnostic
    ///     information.
    /// </param>
    public Bootstrapper(
        IEnumerable<IStep> steps,
        ILogger<Bootstrapper> logger) {
        _steps = [.. steps];
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken) {
        _logger.BootstrapperStarting();

        var sw = Stopwatch.StartNew();
        var currentStep = 0;
        var totalSteps = _steps.Length;

        foreach (var step in _steps) {
            try { await ExecuteStepAsync(step, context, ++currentStep, totalSteps, cancellationToken).SkipContextSync(); }
            catch (Exception ex) {
                _logger.BootstrapperException(currentStep, totalSteps, sw.ElapsedMilliseconds, ex);

                throw;
            }

            if (!cancellationToken.IsCancellationRequested) {
                continue;
            }

            _logger.BootstrapperCancelled(currentStep, totalSteps, sw.ElapsedMilliseconds);

            return;
        }

        _logger.BootstrapperSuccess(currentStep, totalSteps, sw.ElapsedMilliseconds);
    }

    private async Task ExecuteStepAsync(IStep step, FlowContext context, int currentStep, int totalSteps, CancellationToken cancellationToken) {
        _logger.StepStarting(step, currentStep, totalSteps);

        var sw = Stopwatch.StartNew();

        try { await step.ExecuteAsync(context, cancellationToken).SkipContextSync(); }
        catch (OperationCanceledException) {
            _logger.StepCancelled(step, currentStep, totalSteps, sw.ElapsedMilliseconds);

            return;
        }
        catch (Exception ex) {
            _logger.StepException(step, currentStep, totalSteps, sw.ElapsedMilliseconds, ex);

            if (step.ThrowOnError) {
                throw;
            }
        }

        _logger.StepSuccess(step, currentStep, totalSteps, sw.ElapsedMilliseconds);
    }
}