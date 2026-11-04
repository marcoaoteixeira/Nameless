using Nameless.Mediator.Requests;

namespace Nameless.WinApp.Features.CheckSystemUpdate;

/// <summary>
///     Request to check GitHub Releases for a newer <c>winapp-v*</c> release
///     than the one currently running (release contract §1, §3).
/// </summary>
public sealed class CheckSystemUpdateRequest : IRequest<SystemUpdateCheckResult>;
