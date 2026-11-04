using System.IO;
using Nameless.Application;
using Nameless.Contracts;
using Nameless.Mediator.Requests;
using Nameless.ObjectModel;
using Nameless.WinApp.Infrastructure.GitHub;

namespace Nameless.WinApp.Features.CheckSystemUpdate;

/// <summary>
///     Default handler for <see cref="CheckSystemUpdateRequest"/>.
/// </summary>
public class CheckSystemUpdateRequestHandler : IRequestHandler<CheckSystemUpdateRequest, SystemUpdateCheckResult> {
    private readonly IGitHubReleaseClient _gitHubReleaseClient;
    private readonly IApplicationContext _applicationContext;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="CheckSystemUpdateRequestHandler"/> class.
    /// </summary>
    /// <param name="gitHubReleaseClient">
    ///     The GitHub releases client.
    /// </param>
    /// <param name="applicationContext">
    ///     The application context.
    /// </param>
    public CheckSystemUpdateRequestHandler(IGitHubReleaseClient gitHubReleaseClient, IApplicationContext applicationContext) {
        _gitHubReleaseClient = Throws.When.Null(gitHubReleaseClient);
        _applicationContext = Throws.When.Null(applicationContext);
    }

    /// <inheritdoc />
    public async Task<SystemUpdateCheckResult> HandleAsync(CheckSystemUpdateRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        var current = SemVersion.Parse(_applicationContext.Version);

        var releases = await _gitHubReleaseClient.ListReleasesAsync(cancellationToken);

        var newest = PickNewestEligibleRelease(releases, out var newestVersion);

        if (newest is null || newestVersion is null || newestVersion <= current) {
            return new SystemUpdateCheckResult(current, Release: null);
        }

        var availableRelease = BuildAvailableRelease(newest, newestVersion);

        return new SystemUpdateCheckResult(current, availableRelease);
    }

    // Release contract §1: only non-draft, non-prerelease releases whose
    // tag starts with the WinApp prefix and whose remainder parses as a
    // SemVersion are eligible. Among those, the highest SemVersion wins —
    // never the list order and never the newest by date.
    private static GitHubRelease? PickNewestEligibleRelease(IReadOnlyList<GitHubRelease> releases, out SemVersion? newestVersion) {
        GitHubRelease? newest = null;
        newestVersion = null;

        foreach (var release in releases) {
            if (release.Draft || release.Prerelease) { continue; }

            if (!release.TagName.StartsWith(Constants.SystemUpdate.TagPrefix, StringComparison.Ordinal)) { continue; }

            var versionPart = release.TagName[Constants.SystemUpdate.TagPrefix.Length..];

            if (!SemVersion.TryParse(versionPart, out var version)) { continue; }

            if (newestVersion is null || version > newestVersion) {
                newestVersion = version;
                newest = release;
            }
        }

        return newest;
    }

    // Release contract §2 and §4.1: the newest eligible release must carry
    // exactly one package asset and one checksum sidecar, within the size
    // cap, served from the repository's exact download prefix. A failure
    // here never falls back to an older release — a malformed newest
    // release is a publishing bug to surface, not to hide.
    private static AvailableRelease BuildAvailableRelease(GitHubRelease release, SemVersion version) {
        var packageName = string.Format(Constants.SystemUpdate.PackageNamePattern, version.Format());
        var checksumName = packageName + Constants.SystemUpdate.ChecksumSuffix;

        var package = FindExactlyOneAsset(release, packageName);
        var checksum = FindExactlyOneAsset(release, checksumName);

        if (package.Size <= 0 || package.Size > Constants.SystemUpdate.MaxPackageBytes) {
            throw new InvalidDataException(
                $"Release '{release.TagName}' is malformed: package size {package.Size} is outside the allowed range (1..{Constants.SystemUpdate.MaxPackageBytes} bytes)."
            );
        }

        var expectedPrefix = $"https://github.com/{Constants.SystemUpdate.RepositoryOwner}/{Constants.SystemUpdate.RepositoryName}/releases/download/{release.TagName}/";

        EnsureValidDownloadUrl(release.TagName, package.DownloadUrl, expectedPrefix);
        EnsureValidDownloadUrl(release.TagName, checksum.DownloadUrl, expectedPrefix);

        return new AvailableRelease(
            Version: version,
            TagName: release.TagName,
            HtmlUrl: release.HtmlUrl,
            Package: new ReleaseAsset(package.Name, package.Size, package.DownloadUrl),
            Checksum: new ReleaseAsset(checksum.Name, checksum.Size, checksum.DownloadUrl)
        );
    }

    private static GitHubReleaseAsset FindExactlyOneAsset(GitHubRelease release, string assetName) {
        var matches = release.Assets.Where(asset => string.Equals(asset.Name, assetName, StringComparison.Ordinal)).ToArray();

        if (matches.Length != 1) {
            throw new InvalidDataException(
                $"Release '{release.TagName}' is malformed: expected exactly one asset named '{assetName}', found {matches.Length}."
            );
        }

        return matches[0];
    }

    private static void EnsureValidDownloadUrl(string tagName, Uri url, string expectedPrefix) {
        if (!string.Equals(url.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal) ||
            !url.ToString().StartsWith(expectedPrefix, StringComparison.Ordinal)) {
            throw new InvalidDataException(
                $"Release '{tagName}' is malformed: download URL '{url}' does not start with the expected prefix '{expectedPrefix}'."
            );
        }
    }
}
