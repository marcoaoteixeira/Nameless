using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.WPF.GitHub.ObjectModel;

namespace Nameless.WPF.GitHub.Responses;

public class GetReleaseAssetsResponse : Result<ReleaseAsset[]> {
    private GetReleaseAssetsResponse(ReleaseAsset[] value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator GetReleaseAssetsResponse(ReleaseAsset[] value) {
        return new GetReleaseAssetsResponse(value, errors: []);
    }

    public static implicit operator GetReleaseAssetsResponse(Error error) {
        return new GetReleaseAssetsResponse(value: [], errors: [error]);
    }
}