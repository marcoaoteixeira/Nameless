using Nameless.Mediator.Requests;
using Nameless.WinApp.Features.CheckSystemUpdate;

namespace Nameless.WinApp.Features.PerformSystemUpdate;

/// <summary>
///     Request to download, verify and install the given
///     <paramref name="Release"/> in place of the currently running
///     application (ADR-001).
/// </summary>
/// <param name="Release">
///     The release to install, as validated by
///     <see cref="CheckSystemUpdateRequestHandler"/>.
/// </param>
public sealed record PerformSystemUpdateRequest(AvailableRelease Release) : IRequest<SystemUpdateOutcome>;
