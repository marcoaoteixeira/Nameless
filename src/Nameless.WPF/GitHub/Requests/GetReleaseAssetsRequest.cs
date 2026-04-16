namespace Nameless.WPF.GitHub.Requests;

public record GetReleaseAssetsRequest(string Owner, string Repository, int ReleaseID);