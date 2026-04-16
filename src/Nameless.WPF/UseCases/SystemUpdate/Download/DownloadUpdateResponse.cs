using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.WPF.UseCases.SystemUpdate.Download;

public class DownloadUpdateResponse : Result<DownloadUpdateMetadata> {
    private DownloadUpdateResponse(DownloadUpdateMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator DownloadUpdateResponse(DownloadUpdateMetadata value) {
        return new DownloadUpdateResponse(value, errors: []);
    }

    public static implicit operator DownloadUpdateResponse(Error error) {
        return new DownloadUpdateResponse(value: default, errors: [error]);
    }
}