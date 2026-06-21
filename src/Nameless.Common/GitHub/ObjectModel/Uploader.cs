using System.Text.Json.Serialization;

namespace Nameless.GitHub.ObjectModel;

/// <summary>
///     Uploader object
/// </summary>
/// <param name="Login">The uploader login</param>
/// <param name="Id">The uploader ID</param>
/// <param name="NodeId">The node ID</param>
/// <param name="AvatarUrl">The uploader avatar URL</param>
/// <param name="GravatarId">The Gravatar ID</param>
/// <param name="Url">The URL to the resource</param>
/// <param name="HtmlUrl">The HTML page URL</param>
/// <param name="FollowersUrl">The followers page URL</param>
/// <param name="FollowingUrl">The following page URL</param>
/// <param name="GistsUrl">The Gists page URL</param>
/// <param name="StarredUrl">The starred page URL</param>
/// <param name="SubscriptionsUrl">The subscriptions page URL</param>
/// <param name="OrganizationsUrl">The organizations page URL</param>
/// <param name="ReposUrl">The repositories page URL</param>
/// <param name="EventsUrl">The events page URL</param>
/// <param name="ReceivedEventsUrl">The received events page URL</param>
/// <param name="Type">The type</param>
/// <param name="UserViewType">The user view type</param>
/// <param name="SiteAdmin">The site administrator</param>
public record Uploader(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("node_id")] string NodeId,
    [property: JsonPropertyName("avatar_url")] string AvatarUrl,
    [property: JsonPropertyName("gravatar_id")] string GravatarId,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("followers_url")] string FollowersUrl,
    [property: JsonPropertyName("following_url")] string FollowingUrl,
    [property: JsonPropertyName("gists_url")] string GistsUrl,
    [property: JsonPropertyName("starred_url")] string StarredUrl,
    [property: JsonPropertyName("subscriptions_url")] string SubscriptionsUrl,
    [property: JsonPropertyName("organizations_url")] string OrganizationsUrl,
    [property: JsonPropertyName("repos_url")] string ReposUrl,
    [property: JsonPropertyName("events_url")] string EventsUrl,
    [property: JsonPropertyName("received_events_url")] string ReceivedEventsUrl,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("user_view_type")] string UserViewType,
    [property: JsonPropertyName("site_admin")] bool SiteAdmin
);