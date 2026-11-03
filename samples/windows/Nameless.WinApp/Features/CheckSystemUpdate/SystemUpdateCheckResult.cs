using Nameless.ObjectModel;

namespace Nameless.WinApp.Features.CheckSystemUpdate;

/// <summary>
///     The outcome of a <see cref="CheckSystemUpdateRequest"/>.
/// </summary>
/// <param name="Current">
///     The currently running version.
/// </param>
/// <param name="Release">
///     The newest eligible release, when one is available and strictly
///     higher than <see cref="Current"/>; otherwise <see langword="null"/>.
/// </param>
public sealed record SystemUpdateCheckResult(SemVersion Current, AvailableRelease? Release) {
    /// <summary>
    ///     Whether an update is available.
    /// </summary>
    public bool IsUpdateAvailable => Release is not null;
}
