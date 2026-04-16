using Nameless.Mediator.Requests;

namespace Nameless.WPF.UseCases.SystemUpdate.Fetch;

public record FetchNewVersionInformationRequest(
    int ReleaseID,
    string ApplicationName,
    string Version
) : IRequest<FetchNewVersionInformationResponse>;