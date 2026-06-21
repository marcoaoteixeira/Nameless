using Nameless.GitHub.ObjectModel;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.GitHub.Responses;

/// <summary>
///     Represents a response to a request for the release assets
/// </summary>
public class GetReleaseAssetsResponse : Result<ReleaseAsset[]> {
    private GetReleaseAssetsResponse(ReleaseAsset[] value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts the <see cref="ReleaseAsset"/> array instance into a
    ///     <see cref="GetReleaseAssetsResponse"/> object.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="ReleaseAsset"/> array instance.
    /// </param>
    public static implicit operator GetReleaseAssetsResponse(ReleaseAsset[] value) {
        return new GetReleaseAssetsResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts the <see cref="Error"/> object instance into a
    ///     <see cref="GetReleaseAssetsResponse"/> object.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> object instance.
    /// </param>
    public static implicit operator GetReleaseAssetsResponse(Error error) {
        return new GetReleaseAssetsResponse(value: [], errors: [error]);
    }
}