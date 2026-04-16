using System.Text.Json.Serialization;

namespace Nameless.WPF.GitHub.ObjectModel;

/// <summary>
///     Defines an author.
/// </summary>
/// <param name="Login">The author login.</param>
/// <param name="Id">The author id.</param>
/// <param name="NodeId">The GitHub node id for the author.</param>
/// <param name="AvatarUrl">The author avatar URL.</param>
/// <param name="GravatarId">The author Gravatar ID.</param>
/// <param name="Url">The GitHub API URL to the author information.</param>
/// <param name="HtmlUrl">The GitHub URL for the author.</param>
/// <param name="FollowersUrl">Followers URL.</param>
/// <param name="FollowingUrl">Following URL.</param>
/// <param name="GistsUrl">Gists URL.</param>
/// <param name="StarredUrl">Starred Projects URL.</param>
/// <param name="SubscriptionsUrl">Subscriptions URL.</param>
/// <param name="OrganizationsUrl">Organizations URL.</param>
/// <param name="ReposUrl">Repositories URL.</param>
/// <param name="EventsUrl">Events URL.</param>
/// <param name="ReceivedEventsUrl">Received Events URL.</param>
/// <param name="Type">Author type.</param>
/// <param name="UserViewType">User view type.</param>
/// <param name="SiteAdmin">Whether it is the site administrator.</param>
public record Author(
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