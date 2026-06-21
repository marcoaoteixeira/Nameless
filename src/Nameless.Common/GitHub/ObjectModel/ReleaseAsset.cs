using System.Text.Json.Serialization;

namespace Nameless.GitHub.ObjectModel;

/// <summary>
///     Release asset object
/// </summary>
/// <param name="Url">The release asset URL</param>
/// <param name="Id">The release asset ID</param>
/// <param name="NodeId">The node ID</param>
/// <param name="Name">The name</param>
/// <param name="Label">The label</param>
/// <param name="Uploader">The uploader</param>
/// <param name="ContentType">The content type of the release asset</param>
/// <param name="State">The state</param>
/// <param name="Size">The size in Kb</param>
/// <param name="Digest">The digest</param>
/// <param name="DownloadCount">The download count</param>
/// <param name="CreatedAt">The creation date</param>
/// <param name="UpdatedAt">The modification date</param>
/// <param name="BrowserDownloadUrl">The browser download URL</param>
public record ReleaseAsset(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("node_id")] string NodeId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("uploader")] Uploader Uploader,
    [property: JsonPropertyName("content_type")] string ContentType,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("digest")] object Digest,
    [property: JsonPropertyName("download_count")] int DownloadCount,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
    [property: JsonPropertyName("browser_download_url")] string BrowserDownloadUrl
);