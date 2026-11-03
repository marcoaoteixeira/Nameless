using System.Text.Json.Serialization;

namespace Nameless.WinApp.Infrastructure.GitHub;

/// <summary>
///     A single asset attached to a GitHub release. Only the fields this
///     feature reads (release contract §4.1) are mapped; every other field
///     coming from GitHub is ignored by <see cref="System.Text.Json"/>'s
///     default unknown-member handling.
/// </summary>
/// <param name="Name">
///     The asset file name, as uploaded.
/// </param>
/// <param name="Size">
///     The asset size, in bytes.
/// </param>
/// <param name="DownloadUrl">
///     The direct download URL.
/// </param>
public sealed record GitHubReleaseAsset(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("browser_download_url")] Uri DownloadUrl);
