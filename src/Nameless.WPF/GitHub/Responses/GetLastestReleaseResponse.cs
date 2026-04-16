using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.WPF.GitHub.ObjectModel;

namespace Nameless.WPF.GitHub.Responses;

public class GetLastestReleaseResponse : Result<Release> {
    private GetLastestReleaseResponse(Release? value, Error[] errors)
        : base (value, errors) { }

    public static implicit operator GetLastestReleaseResponse(Release value) {
        return new GetLastestReleaseResponse(value, errors: []);
    }

    public static implicit operator GetLastestReleaseResponse(Error error) {
        return new GetLastestReleaseResponse(value: null, errors: [error]);
    }
}