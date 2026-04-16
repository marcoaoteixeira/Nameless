using Microsoft.Extensions.Options;
using Nameless.Application;
using Nameless.Mediator.Requests;
using Nameless.WPF.GitHub;
using Nameless.WPF.GitHub.Requests;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.UseCases.SystemUpdate.Check;

public class CheckForUpdateRequestHandler : IRequestHandler<CheckForUpdateRequest, CheckForUpdateResponse> {
    private static readonly SemanticVersion EmptyVersion = new(major: 0, minor: 0, patch: 0);
    
    private readonly IApplicationContext _applicationContext;
    private readonly IGitHubHttpClient _gitHubHttpClient;
    private readonly IMessenger _messenger;
    private readonly IOptions<GitHubOptions> _options;

    public CheckForUpdateRequestHandler(IApplicationContext applicationContext, IGitHubHttpClient gitHubHttpClient, IMessenger messenger, IOptions<GitHubOptions> options) {
        _applicationContext = applicationContext;
        _gitHubHttpClient = gitHubHttpClient;
        _messenger = messenger;
        _options = options;
    }

    public async Task<CheckForUpdateResponse> HandleAsync(CheckForUpdateRequest request, CancellationToken cancellationToken) {
        await _messenger.NotifyStartingAsync()
                               .SkipContextSync();

        var options = _options.Value;
        var getLatestReleaseRequest = new GetLastestReleaseRequest(options.Owner, options.Repository);
        var getLastestReleaseResponse = await _gitHubHttpClient.GetLastestReleaseAsync(getLatestReleaseRequest, cancellationToken)
                                                               .SkipContextSync();

        if (getLastestReleaseResponse.Failure) {
            await _messenger.NotifyFailureAsync(getLastestReleaseResponse.Errors[0].Message)
                                   .SkipContextSync();

            return getLastestReleaseResponse.Errors;
        }

        if (!SemanticVersion.TryParse(_applicationContext.Version, out var currentVersion)) {
            currentVersion = EmptyVersion;
        }

        if (!SemanticVersion.TryParse(getLastestReleaseResponse.Value.TagName, out var latestVersion)) {
            latestVersion = EmptyVersion;
        }

        if (currentVersion >= latestVersion) {
            await _messenger.NotifySuccessAsync()
                            .SkipContextSync();

            return (CheckForUpdateMetadata)default;
        }

        await _messenger.NotifySuccessAsync(latestVersion.ToString() ?? string.Empty)
                        .SkipContextSync();

        return new CheckForUpdateMetadata(
            ReleaseID: getLastestReleaseResponse.Value.Id,
            ApplicationName: _applicationContext.ApplicationName,
            Version: latestVersion.ToString()
        );
    }
}