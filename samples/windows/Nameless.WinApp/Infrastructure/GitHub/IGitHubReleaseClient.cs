namespace Nameless.WinApp.Infrastructure.GitHub;

/// <summary>
///     Thin client for the subset of GitHub's Releases REST API the
///     self-update feature needs (release contract §4).
/// </summary>
public interface IGitHubReleaseClient {
    /// <summary>
    ///     Lists the repository's releases (first page, 30 entries).
    /// </summary>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     The releases, in the order GitHub returns them (not assumed to
    ///     be sorted by version).
    /// </returns>
    /// <exception cref="GitHubReleaseClientException">
    ///     When the call fails: network error, timeout, non-2xx response or
    ///     rate limit.
    /// </exception>
    Task<IReadOnlyList<GitHubRelease>> ListReleasesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Downloads <paramref name="url"/> as UTF-8 text, refusing a body
    ///     larger than <paramref name="maxBytes"/>.
    /// </summary>
    /// <param name="url">
    ///     The download URL.
    /// </param>
    /// <param name="maxBytes">
    ///     The maximum number of bytes to read.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <exception cref="GitHubReleaseClientException">
    ///     When the call fails.
    /// </exception>
    /// <exception cref="InvalidDataException">
    ///     When the body exceeds <paramref name="maxBytes"/>.
    /// </exception>
    Task<string> DownloadTextAsync(Uri url, long maxBytes, CancellationToken cancellationToken);

    /// <summary>
    ///     Streams <paramref name="url"/> to <paramref name="destinationPath"/>,
    ///     overwriting it, without buffering the whole body in memory.
    ///     Aborts as soon as more than <paramref name="maxBytes"/> have been
    ///     written, instead of only checking the length after the fact.
    /// </summary>
    /// <param name="url">
    ///     The download URL.
    /// </param>
    /// <param name="destinationPath">
    ///     The destination file path.
    /// </param>
    /// <param name="maxBytes">
    ///     The maximum number of bytes to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <exception cref="GitHubReleaseClientException">
    ///     When the call fails.
    /// </exception>
    /// <exception cref="InvalidDataException">
    ///     When the body exceeds <paramref name="maxBytes"/>.
    /// </exception>
    Task DownloadToFileAsync(Uri url, string destinationPath, long maxBytes, CancellationToken cancellationToken);
}
