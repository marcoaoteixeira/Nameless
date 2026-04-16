using System.Net.Http;
using System.Net.Http.Json;
using Nameless.ObjectModel;
using Nameless.WPF.GitHub.ObjectModel;
using Nameless.WPF.GitHub.Requests;
using Nameless.WPF.GitHub.Responses;
using Nameless.WPF.Resources;

namespace Nameless.WPF.GitHub.Impl;

public class GitHubHttpClient : IGitHubHttpClient {
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Initializes a new instance
    ///     of <see cref="GitHubHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">
    ///     The underlying HTTP client.
    /// </param>
    public GitHubHttpClient(HttpClient httpClient) {
        _httpClient = httpClient;
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

            if (release is null) {
                return Error.Failure(string.Format(Strings.GetLastestReleaseResponse_DeserializationFailure_Message, statusCode));
            }

            return release;
        }
        catch (Exception ex) {
            return Error.Failure(string.Format(Strings.GetLastestReleaseResponse_Failure_Message, statusCode, ex.Message));
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

            if (assets is null) {
                return Error.Failure(string.Format(Strings.GetReleaseAssetsResponse_DeserializationFailure_Message, statusCode));
            }

            return assets;
        }
        catch (Exception ex) {
            return Error.Failure(string.Format(Strings.GetReleaseAssetsResponse_Failure_Message, statusCode, ex.Message));
        }
    }
}
