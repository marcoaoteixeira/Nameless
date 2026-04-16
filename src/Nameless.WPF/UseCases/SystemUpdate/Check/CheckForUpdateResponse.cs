using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.WPF.UseCases.SystemUpdate.Check;

public class CheckForUpdateResponse : Result<CheckForUpdateMetadata> {
    private CheckForUpdateResponse(CheckForUpdateMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator CheckForUpdateResponse(CheckForUpdateMetadata value) {
        return new CheckForUpdateResponse(value, errors: []);
    }

    public static implicit operator CheckForUpdateResponse(Error[] errors) {
        return new CheckForUpdateResponse(value: default, errors);
    }

    public static implicit operator CheckForUpdateResponse(Error error) {
        return new CheckForUpdateResponse(value: default, errors: [error]);
    }
}