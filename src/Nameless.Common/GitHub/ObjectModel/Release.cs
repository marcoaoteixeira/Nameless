using System.Text.Json.Serialization;

namespace Nameless.GitHub.ObjectModel;

/// <summary>
///     Release object.
/// </summary>
/// <param name="Url">The release URL</param>
/// <param name="AssetsUrl">The release assets URL</param>
/// <param name="UploadUrl">The upload ULR</param>
/// <param name="HtmlUrl">The release page URL.</param>
/// <param name="Id">The release ID</param>
/// <param name="Author">The author</param>
/// <param name="NodeId">The node ID</param>
/// <param name="TagName">The tag name</param>
/// <param name="TargetCommitish">The target commitish</param>
/// <param name="Name">The release name</param>
/// <param name="Draft">Whether it is a draft</param>
/// <param name="Immutable">Whether it is immutable</param>
/// <param name="Prerelease">The pre-release value</param>
/// <param name="CreatedAt">The creation date</param>
/// <param name="UpdatedAt">The modification date</param>
/// <param name="PublishedAt">The publishing date</param>
/// <param name="Assets">The assets list</param>
/// <param name="TarballUrl">The tarball URL</param>
/// <param name="ZipballUrl">The zip-ball URL</param>
/// <param name="Body">The body</param>
public record Release(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("assets_url")] string AssetsUrl,
    [property: JsonPropertyName("upload_url")] string UploadUrl,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("author")] Author Author,
    [property: JsonPropertyName("node_id")] string NodeId,
    [property: JsonPropertyName("tag_name")] string TagName,
    [property: JsonPropertyName("target_commitish")] string TargetCommitish,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("draft")] bool Draft,
    [property: JsonPropertyName("immutable")] bool Immutable,
    [property: JsonPropertyName("prerelease")] bool Prerelease,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
    [property: JsonPropertyName("published_at")] DateTime PublishedAt,
    [property: JsonPropertyName("assets")] IReadOnlyList<object> Assets,
    [property: JsonPropertyName("tarball_url")] string TarballUrl,
    [property: JsonPropertyName("zipball_url")] string ZipballUrl,
    [property: JsonPropertyName("body")] string Body
);