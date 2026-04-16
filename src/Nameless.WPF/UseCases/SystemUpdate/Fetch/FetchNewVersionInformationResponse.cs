using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.WPF.UseCases.SystemUpdate.Fetch;

public class FetchNewVersionInformationResponse : Result<FetchNewVersionMetadata> {
    private FetchNewVersionInformationResponse(FetchNewVersionMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator FetchNewVersionInformationResponse(FetchNewVersionMetadata value) {
        return new FetchNewVersionInformationResponse(value, errors: []);
    }

    public static implicit operator FetchNewVersionInformationResponse(Error error) {
        return new FetchNewVersionInformationResponse(value: default, errors: [error]);
    }
}