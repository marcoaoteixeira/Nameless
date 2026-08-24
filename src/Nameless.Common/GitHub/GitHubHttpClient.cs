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
    private const string LOG_TAG = "GITHUB_HTTP_CLIENT";

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
                : UnableDeserializeResponse(nameof(Release), statusCode);
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LOG_TAG);

            return Error.Failure(
                $"An error has occurred while retrieving information about the latest release. Message: {ex.Message} | Status code: {statusCode}"
            );
        }
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
                : UnableDeserializeResponse(nameof(ReleaseAsset), statusCode);
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LOG_TAG);

            return Error.Failure(
                $"An error has occurred while retrieving information about release assets. Message: {ex.Message} | Status code: {statusCode}"
            );
        }
    }

    private Error UnableDeserializeResponse(string objectName, int statusCode) {
        var reason = $"Unable to deserialize object '{objectName}' from response content. Status code: {statusCode}";

        CommonLog.Warning(_logger, reason, tag: LOG_TAG);

        return Error.Conflict(reason);
    }
}