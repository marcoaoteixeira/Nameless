namespace Nameless.WinApp.Infrastructure.GitHub;

/// <summary>
///     Raised when a call to the GitHub Releases API fails: a network
///     failure, a timeout, a non-2xx response, or the unauthenticated rate
///     limit being exhausted.
/// </summary>
public sealed class GitHubReleaseClientException : Exception {
    /// <summary>
    ///     Whether this failure is GitHub's unauthenticated rate limit
    ///     (60 requests/hour per IP) being exhausted.
    /// </summary>
    public bool IsRateLimited { get; }

    /// <summary>
    ///     When the rate limit resets, if known and <see cref="IsRateLimited"/>.
    /// </summary>
    public DateTimeOffset? RateLimitResetsAt { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="GitHubReleaseClientException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="isRateLimited">
    ///     Whether the failure is a rate-limit response.
    /// </param>
    /// <param name="rateLimitResetsAt">
    ///     When the rate limit resets, if known.
    /// </param>
    /// <param name="innerException">
    ///     The underlying exception, if any.
    /// </param>
    public GitHubReleaseClientException(
        string message,
        bool isRateLimited = false,
        DateTimeOffset? rateLimitResetsAt = null,
        Exception? innerException = null) : base(message, innerException) {
        IsRateLimited = isRateLimited;
        RateLimitResetsAt = rateLimitResetsAt;
    }
}
