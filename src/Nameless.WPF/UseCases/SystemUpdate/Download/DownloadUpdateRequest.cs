using Nameless.Mediator.Requests;

namespace Nameless.WPF.UseCases.SystemUpdate.Download;

public record DownloadUpdateRequest(
    string Version,
    string Url
) : IRequest<DownloadUpdateResponse>;