using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Nameless.GitHub.ObjectModel;
using Nameless.GitHub.Requests;
using Nameless.GitHub.Responses;
using Nameless.ObjectModel;

namespace Nameless.GitHub;

/// <summary>
///     Current implementation of <see cref="IGitHubHttpClient"/>
/// </summary>
public class GitHubHttpClient : IGitHubHttpClient {
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubHttpClient> _logger;

    /// <summary>
    ///     Initializes a new instance
    ///     of <see cref="GitHubHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">
    ///     The underlying HTTP client.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public GitHubHttpClient(HttpClient httpClient, ILogger<GitHubHttpClient> logger) {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<GetLastestReleaseResponse> GetLastestReleaseAsync(GetLastestReleaseRequest request, CancellationToken cancellationToken) {
        var statusCode = 200;

        try {
            var url = $"/repos/{request.Owner}/{request.Repository}/releases/latest";
            var response = await _httpClient.GetAsync(url, cancellationToken)
                                            .SkipContextSync();

            statusCode = (int)response.StatusCode;

            response.EnsureSuccessStatusCode();

            var release = await response.Content
                                        .ReadFromJsonAsync<Release>(cancellationToken)
                                        .SkipContextSync();

            return release is not null
                ? release
                : GetLastestReleaseDeserializationFailureResponse(statusCode);
        }
        catch (Exception ex) { return GetLastestReleaseUnknownFailureResponse(statusCode, ex); }
    }

    /// <inheritdoc />
    public async Task<GetReleaseAssetsResponse> GetReleaseAssetsAsync(GetReleaseAssetsRequest request, CancellationToken cancellationToken) {
        var statusCode = 200;

        try {
            var url = $"/repos/{request.Owner}/{request.Repository}/releases/{request.ReleaseID}/assets";
            var response = await _httpClient.GetAsync(url, cancellationToken)
                                            .SkipContextSync();

            statusCode = (int)response.StatusCode;

            response.EnsureSuccessStatusCode();

            var assets = await response.Content
                                       .ReadFromJsonAsync<ReleaseAsset[]>(cancellationToken)
                                       .SkipContextSync();

            return assets is not null
                ? assets
                : GetReleaseAssetsDeserializationFailureResponse(statusCode);
        }
        catch (Exception ex) { return GetReleaseAssetsUnknownFailureResponse(statusCode, ex); }
    }

    private Error GetLastestReleaseDeserializationFailureResponse(int statusCode) {
        var message = $"Couldn't deserialize object '{nameof(Release)}' from response content. Status code: {statusCode}";

        _logger.Warning(nameof(GetLastestReleaseAsync), message);

        return Error.Conflict(message);
    }

    private Error GetLastestReleaseUnknownFailureResponse(int statusCode, Exception exception) {
        var message = $"An error has occurred while executing call '{nameof(GetLastestReleaseAsync)}' in '{nameof(GitHubHttpClient)}'. Status code: {statusCode}. Reason: {exception.Message}";

        _logger.Failure(nameof(GetLastestReleaseAsync), exception);

        return Error.Failure(message);
    }

    private Error GetReleaseAssetsDeserializationFailureResponse(int statusCode) {
        var message = $"Couldn't deserialize array of '{nameof(ReleaseAsset)}' from response content. Status code: {statusCode}";

        _logger.Warning(nameof(GetReleaseAssetsAsync), message);

        return Error.Conflict(message);
    }

    private Error GetReleaseAssetsUnknownFailureResponse(int statusCode, Exception exception) {
        var message = $"An error has occurred while executing call '{nameof(GetReleaseAssetsAsync)}' in '{nameof(GitHubHttpClient)}'. Status code: {statusCode}. Reason: {exception.Message}";

        _logger.Failure(nameof(GetReleaseAssetsAsync), exception);

        return Error.Failure(message);
    }
}
