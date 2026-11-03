using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Nameless.Contracts;

namespace Nameless.WinApp.Infrastructure.GitHub;

/// <summary>
///     Default <see cref="IGitHubReleaseClient"/> implementation, backed by
///     a typed <see cref="HttpClient"/> registered by
///     <see cref="SystemUpdateHostFeature"/>.
/// </summary>
public sealed class GitHubReleaseClient : IGitHubReleaseClient {
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Initializes a new instance of <see cref="GitHubReleaseClient"/> class.
    /// </summary>
    /// <param name="httpClient">
    ///     The typed <see cref="HttpClient"/>, pre-configured with the
    ///     GitHub API base address and headers.
    /// </param>
    public GitHubReleaseClient(HttpClient httpClient) {
        _httpClient = Throws.When.Null(httpClient);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GitHubRelease>> ListReleasesAsync(CancellationToken cancellationToken) {
        var requestUri = $"repos/{Constants.SystemUpdate.RepositoryOwner}/{Constants.SystemUpdate.RepositoryName}/releases?per_page=30&page=1";

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);

        var releases = await response.Content.ReadFromJsonAsync<IReadOnlyList<GitHubRelease>>(cancellationToken);

        return releases ?? [];
    }

    /// <inheritdoc />
    public async Task<string> DownloadTextAsync(Uri url, long maxBytes, CancellationToken cancellationToken) {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        return await ReadBoundedTextAsync(response.Content, maxBytes, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DownloadToFileAsync(Uri url, string destinationPath, CancellationToken cancellationToken) {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var destination = SysFile.Create(destinationPath);

        await source.CopyToAsync(destination, cancellationToken);
    }

    private static async Task<string> ReadBoundedTextAsync(HttpContent content, long maxBytes, CancellationToken cancellationToken) {
        await using var source = await content.ReadAsStreamAsync(cancellationToken);
        using var buffer = new MemoryStream();

        var chunk = new byte[4096];
        long total = 0;
        int read;

        while ((read = await source.ReadAsync(chunk, cancellationToken)) > 0) {
            total += read;

            if (total > maxBytes) {
                throw new InvalidDataException($"Response body exceeded the {maxBytes}-byte cap.");
            }

            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken) {
        HttpResponseMessage response;

        try {
            response = await _httpClient.SendAsync(request, completionOption, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) {
            // HttpClient.Timeout surfaces as a TaskCanceledException (an
            // OperationCanceledException) even though the caller's token
            // was never signaled — the check above tells the two apart.
            throw new GitHubReleaseClientException("The request to GitHub timed out.");
        }
        catch (HttpRequestException ex) {
            throw new GitHubReleaseClientException($"Could not reach GitHub: {ex.Message}", innerException: ex);
        }

        if (response.IsSuccessStatusCode) { return response; }

        throw await BuildFailureExceptionAsync(response, cancellationToken);
    }

    private static async Task<GitHubReleaseClientException> BuildFailureExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken) {
        using (response) {
            var isRateLimited = response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests
                && response.Headers.TryGetValues("x-ratelimit-remaining", out var remainingValues)
                && remainingValues.FirstOrDefault() == "0";

            if (isRateLimited) {
                DateTimeOffset? resetsAt = null;

                if (response.Headers.TryGetValues("x-ratelimit-reset", out var resetValues)
                    && long.TryParse(resetValues.FirstOrDefault(), out var unixSeconds)) {
                    resetsAt = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
                }

                return new GitHubReleaseClientException(
                    message: "GitHub's request limit was reached.",
                    isRateLimited: true,
                    rateLimitResetsAt: resetsAt
                );
            }

            // Best-effort: the body may carry GitHub's error message, but
            // a failure to read it must not mask the status-code failure.
            string? body = null;

            try { body = await response.Content.ReadAsStringAsync(cancellationToken); }
            catch (Exception) { /* best effort */ }

            var suffix = string.IsNullOrWhiteSpace(body) ? string.Empty : $" {body}";

            return new GitHubReleaseClientException($"GitHub responded with status code {(int)response.StatusCode}.{suffix}");
        }
    }
}
