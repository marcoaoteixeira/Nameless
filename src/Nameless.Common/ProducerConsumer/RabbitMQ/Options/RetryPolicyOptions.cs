using Nameless.Resilience;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     Retry Policy Options.
/// </summary>
public record RetryPolicyOptions {
    /// <summary>
    ///     Gets the maximum number of retries.
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    ///     Gets the retry initial delay.
    /// </summary>
    public TimeSpan InitialDelay { get; init; }

    /// <summary>
    ///     Gets the backoff type.
    /// </summary>
    public BackoffType BackoffType { get; init; }

    /// <summary>
    ///     Gets the maximum delay between retries.
    /// </summary>
    public TimeSpan MaxDelay { get; init; }

    /// <summary>
    ///     Whether it should use jitter for delay.
    /// </summary>
    public bool UseJitter { get; init; }

    internal RetryPolicyConfiguration CreateConfiguration(string consumerTag, Action<Exception?, TimeSpan, int, int> onRetry, Func<Exception, bool>? onRetryException = null) {
        return new RetryPolicyConfiguration {
            Tag = consumerTag,
            RetryCount = RetryCount >= 0 ? RetryCount : 0,
            InitialDelay = InitialDelay >= TimeSpan.Zero ? InitialDelay : TimeSpan.Zero,
            BackoffType = BackoffType,
            MaxDelay = MaxDelay >= TimeSpan.Zero ? MaxDelay : TimeSpan.Zero,
            UseJitter = UseJitter,
            RetryOnException = onRetryException,
            OnRetry = onRetry
        };
    }
}
