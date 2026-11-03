using System.Text.Json.Serialization;

namespace Nameless.WinApp.Infrastructure.GitHub;

/// <summary>
///     A single entry returned by GitHub's "list releases" endpoint. Only
///     the fields this feature reads (release contract §4.1) are mapped.
/// </summary>
/// <param name="TagName">
///     The git tag name, e.g. <c>winapp-v1.4.0</c>.
/// </param>
/// <param name="Draft">
///     Whether the release is still a draft.
/// </param>
/// <param name="Prerelease">
///     Whether the release is flagged as a pre-release.
/// </param>
/// <param name="HtmlUrl">
///     The release's web page, shown to the user as "release notes".
/// </param>
/// <param name="Assets">
///     The release's uploaded assets.
/// </param>
public sealed record GitHubRelease(
    [property: JsonPropertyName("tag_name")] string TagName,
    [property: JsonPropertyName("draft")] bool Draft,
    [property: JsonPropertyName("prerelease")] bool Prerelease,
    [property: JsonPropertyName("html_url")] Uri? HtmlUrl,
    [property: JsonPropertyName("assets")] IReadOnlyList<GitHubReleaseAsset> Assets);
