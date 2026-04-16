using System.IO;
using System.Net.Http;
using Nameless.IO.FileSystem;
using Nameless.Mediator.Requests;
using Nameless.ObjectModel;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.UseCases.SystemUpdate.Download;

public class DownloadUpdateRequestHandler : IRequestHandler<DownloadUpdateRequest, DownloadUpdateResponse> {
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly HttpClient _httpClient;
    private readonly IMessenger _messenger;
    private readonly TimeProvider _timeProvider;

    public DownloadUpdateRequestHandler(IFileSystemProvider fileSystemProvider, HttpClient httpClient, IMessenger messenger, TimeProvider timeProvider) {
        _fileSystemProvider = fileSystemProvider;
        _httpClient = httpClient;
        _messenger = messenger;
        _timeProvider = timeProvider;
    }

    public async Task<DownloadUpdateResponse> HandleAsync(DownloadUpdateRequest request, CancellationToken cancellationToken) {
        try {
            await _messenger.NotifyStartingAsync()
                                   .SkipContextSync();

            var response = await _httpClient.GetAsync(request.Url, cancellationToken)
                                            .SkipContextSync();

            response.EnsureSuccessStatusCode();

            // Ensure "updates" directory exists
            _fileSystemProvider.GetDirectory(WPFConstants.FolderStructure.UpdateDirectoryName).Create();

            var fileName = $"{_timeProvider.GetUtcNow():yyyyMMddHHmmss}_v{request.Version}.zip";
            var filePath = Path.Combine(WPFConstants.FolderStructure.UpdateDirectoryName, fileName);
            var file = _fileSystemProvider.GetFile(filePath);

            await _messenger.NotifyWritingFileAsync()
                                   .SkipContextSync();

            await using var fileStream = file.Open();
            await using var httpStream = await response.Content
                                                       .ReadAsStreamAsync(cancellationToken)
                                                       .SkipContextSync();

            await httpStream.CopyToAsync(fileStream, cancellationToken)
                            .SkipContextSync();

            httpStream.Close();
            fileStream.Close();

            await _messenger.NotifySuccessAsync(file.Path)
                                   .SkipContextSync();

            return new DownloadUpdateMetadata(file.Path);
        }
        catch (Exception ex) {
            await _messenger.NotifyFailureAsync(ex.Message)
                                   .SkipContextSync();

            return Error.Failure(ex.Message);
        }
    }
}