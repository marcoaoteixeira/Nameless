namespace Nameless.WinApp.Features.CheckSystemUpdate;

/// <summary>
///     A validated release asset (release contract §2): the package zip or
///     its checksum sidecar.
/// </summary>
/// <param name="Name">
///     The asset's exact file name.
/// </param>
/// <param name="Size">
///     The asset size, in bytes.
/// </param>
/// <param name="DownloadUrl">
///     The asset's download URL, already checked against the repository's
///     exact HTTPS download prefix.
/// </param>
public sealed record ReleaseAsset(string Name, long Size, Uri DownloadUrl);
