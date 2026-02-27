using Microsoft.Extensions.Logging;
using Nameless.Bootstrap.Infrastructure;
using Polly;
using Polly.Retry;

namespace Nameless.Bootstrap.Resilience;

public class RetryPolicyFactory : IRetryPolicyFactory {
    private readonly ILogger<RetryPolicyFactory> _logger;

    public RetryPolicyFactory(ILogger<RetryPolicyFactory> logger) {
        _logger = logger;
    }

    public ResiliencePipeline CreateRetryPipeline(string stepName, RetryPolicyConfiguration configuration) {
        if (configuration.RetryCount <= 0) { return ResiliencePipeline.Empty; }

        var strategy = CreateRetryStrategy(stepName, configuration);

        return new ResiliencePipelineBuilder().AddRetry(strategy)
                                              .Build();
    }

    private RetryStrategyOptions CreateRetryStrategy(string stepName, RetryPolicyConfiguration configuration) {
        return new RetryStrategyOptions {
            MaxRetryAttempts = configuration.RetryCount,

            ShouldHandle = new PredicateBuilder().Handle<Exception>(ex => {
                if (configuration.RetryOnException is not null) {
                    return configuration.RetryOnException(ex);
                }

                // Never retry if is a OperationCanceledException.
                return ex is not OperationCanceledException;
            }),
            
            DelayGenerator = args => {
                var delay = CalculateDelay(configuration, args.AttemptNumber);

                if (configuration.UseJitter) {
                    var jitter = Random.Shared.NextDouble() * 0.3; // ±30%
                    var jitterMultiplier = 1.0 + (jitter - 0.15);

                    delay = TimeSpan.FromMilliseconds(
                        delay.TotalMilliseconds * jitterMultiplier
                    );
                }

                // Ensure we do not go over maximum delay.
                if (delay > configuration.MaxDelay) {
                    delay = configuration.MaxDelay;
                }

                return ValueTask.FromResult<TimeSpan?>(delay);
            },
            
            OnRetry = args => {
                _logger.WarningOnRetry(
                    stepName,
                    args.AttemptNumber,
                    configuration.RetryCount,
                    args.RetryDelay.TotalMilliseconds,
                    args.Outcome.Exception
                );

                configuration.OnRetry.Invoke(
                    args.Outcome.Exception,
                    args.RetryDelay,
                    args.AttemptNumber,
                    configuration.RetryCount
                );

                return ValueTask.CompletedTask;
            }
        };
    }

    private static TimeSpan CalculateDelay(RetryPolicyConfiguration configuration, int currentAttempt) {
        return configuration.BackoffType switch {
            BackoffType.Constant => configuration.InitialDelay,
            
            BackoffType.Linear => TimeSpan.FromMilliseconds(
                configuration.InitialDelay.TotalMilliseconds * currentAttempt
            ),
            
            BackoffType.Exponential => TimeSpan.FromMilliseconds(
                configuration.InitialDelay.TotalMilliseconds * Math.Pow(2, currentAttempt - 1)
            ),

            _ => configuration.InitialDelay
        };
    }
}
