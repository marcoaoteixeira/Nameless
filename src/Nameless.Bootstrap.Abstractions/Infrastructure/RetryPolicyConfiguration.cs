namespace Nameless.Bootstrap.Infrastructure;

/// <summary>
///     Retry policy configuration
/// </summary>
public record RetryPolicyConfiguration {
    /// <summary>
    ///     Gets or sets the maximum number of retries.
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    ///     Gets or sets the retry initial delay.
    ///     Default is 1 second.
    /// </summary>
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromSeconds(1);

    /// <summary>
    ///     Gets or sets the backoff type.
    ///     Default is <see cref="BackoffType.Exponential"/>.
    /// </summary>
    public BackoffType BackoffType { get; init; } = BackoffType.Exponential;

    /// <summary>
    ///     Gets or sets the maximum delay between retries
    ///     (used with <see cref="BackoffType.Exponential"/>).
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Whether it should use jitter for delay.
    /// </summary>
    public bool UseJitter { get; init; } = true;

    /// <summary>
    ///     Gets or sets a predicate to determine if it should retry on
    ///     a particular exception.
    /// </summary>
    public Func<Exception, bool>? RetryOnException { get; init; }

    /// <summary>
    ///     Gets or sets a callback that will get called before all retries
    ///     attempts.
    /// </summary>
    public Action<Exception?, TimeSpan, int, int> OnRetry { get; init; } = (_, _, _, _) => { };
}