using Nameless.WPF.GitHub.Requests;
using Nameless.WPF.GitHub.Responses;

namespace Nameless.WPF.GitHub;

/// <summary>
///     Defines a GitHub HTTP Client.
/// </summary>
public interface IGitHubHttpClient {
    /// <summary>
    ///     Retrieves the latest release information for the repository.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous action
    ///     execution, where the <c>Result</c> of the task is the latest
    ///     release information.
    /// </returns>
    /// <remarks>
    ///     The latest release is the most recent non-prerelease, non-draft
    ///     release, sorted by the <c>created_at</c> attribute.
    ///     The <c>created_at</c> attribute is the date of the commit used
    ///     for the release, and not the date when the release was drafted
    ///     or published.
    /// </remarks>
    Task<GetLastestReleaseResponse> GetLastestReleaseAsync(GetLastestReleaseRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a list of all assets related to the specified release.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous action
    ///     execution, where the <c>Result</c> of the task is the release
    ///     assets information.
    /// </returns>
    Task<GetReleaseAssetsResponse> GetReleaseAssetsAsync(GetReleaseAssetsRequest request, CancellationToken cancellationToken);
}