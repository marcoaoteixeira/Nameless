using Nameless.GitHub.ObjectModel;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.GitHub.Responses;

/// <summary>
///     Represents a response to a request for the latest release
/// </summary>
public class GetLastestReleaseResponse : Result<Release> {
    private GetLastestReleaseResponse(Release? value, Error[] errors)
        : base (value, errors) { }

    /// <summary>
    ///     Converts the <see cref="Release"/> object instance into a
    ///     <see cref="GetLastestReleaseResponse"/> object.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Release"/> object instance.
    /// </param>
    public static implicit operator GetLastestReleaseResponse(Release value) {
        return new GetLastestReleaseResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts the <see cref="Error"/> object instance into a
    ///     <see cref="GetLastestReleaseResponse"/> object.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> object instance.
    /// </param>
    public static implicit operator GetLastestReleaseResponse(Error error) {
        return new GetLastestReleaseResponse(value: null, errors: [error]);
    }
}