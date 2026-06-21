namespace Nameless.GitHub.Requests;

/// <summary>
///     Represents a request to get the release assets
/// </summary>
/// <param name="Owner">The owner</param>
/// <param name="Repository">The repository name</param>
/// <param name="ReleaseID">The release ID</param>
public record GetReleaseAssetsRequest(string Owner, string Repository, int ReleaseID);