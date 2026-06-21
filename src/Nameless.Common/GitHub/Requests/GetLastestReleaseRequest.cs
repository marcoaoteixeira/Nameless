namespace Nameless.GitHub.Requests;

/// <summary>
///     Represents a request to get the latest release of the repository.
/// </summary>
/// <param name="Owner">The owner</param>
/// <param name="Repository">The repository name</param>
public record GetLastestReleaseRequest(string Owner, string Repository);