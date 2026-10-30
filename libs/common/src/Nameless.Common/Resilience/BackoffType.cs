namespace Nameless.Resilience;

/// <summary>
///     Retry backoff definitions
/// </summary>
public enum BackoffType {
    /// <summary>
    ///     Constant
    /// </summary>
    Constant,

    /// <summary>
    ///     Crescent linear delay (Delay * Attempt number)
    /// </summary>
    Linear,

    /// <summary>
    ///     Exponential delay (Delay * 2 ^ Attempt number)
    /// </summary>
    Exponential
}