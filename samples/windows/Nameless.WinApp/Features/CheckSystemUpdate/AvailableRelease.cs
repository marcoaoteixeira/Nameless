using Nameless.ObjectModel;

namespace Nameless.WinApp.Features.CheckSystemUpdate;

/// <summary>
///     The newest eligible <c>winapp-v*</c> release found by
///     <see cref="CheckSystemUpdateRequestHandler"/>, already validated
///     against the release contract (§2).
/// </summary>
/// <param name="Version">
///     The release's semantic version (the tag without the <c>winapp-v</c>
///     prefix).
/// </param>
/// <param name="TagName">
///     The release's git tag, e.g. <c>winapp-v1.4.0</c>.
/// </param>
/// <param name="HtmlUrl">
///     The release's web page, shown to the user as "release notes".
/// </param>
/// <param name="Package">
///     The package zip asset.
/// </param>
/// <param name="Checksum">
///     The checksum sidecar asset.
/// </param>
public sealed record AvailableRelease(SemVersion Version, string TagName, Uri? HtmlUrl, ReleaseAsset Package, ReleaseAsset Checksum);
