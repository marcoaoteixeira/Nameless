namespace Nameless.Resilience;

/// <summary>
///     Retry configuration
/// </summary>
public record RetryPolicyConfiguration {
    public static RetryPolicyConfiguration Default => new() {
        Tag = $"{Guid.CreateVersion7():N}",
        RetryCount = 3,
        InitialDelay = TimeSpan.FromMilliseconds(250),
        BackoffType = BackoffType.Exponential,
        MaxDelay = TimeSpan.FromSeconds(5),
        UseJitter = true,
        OnRetry = (_, _, _, _) => { }
    };

    /// <summary>
    ///     Gets the tag.
    /// </summary>
    public string? Tag { get; init; }

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

    /// <summary>
    ///     Gets a predicate to determine if it should retry on
    ///     a particular exception.
    /// </summary>
    public Func<Exception, bool>? RetryOnException { get; init; }

    /// <summary>
    ///     Gets a callback that will get called before all retries
    ///     attempts.
    /// </summary>
    public required Action<Exception?, TimeSpan, int, int> OnRetry { get; init; }
}