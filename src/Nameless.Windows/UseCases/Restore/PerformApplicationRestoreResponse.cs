using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Windows.UseCases.Restore;

public class PerformApplicationRestoreResponse : Result<bool> {
    private PerformApplicationRestoreResponse(bool value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator PerformApplicationRestoreResponse(bool value) {
        return new PerformApplicationRestoreResponse(value, errors: []);
    }

    public static implicit operator PerformApplicationRestoreResponse(Error error) {
        return new PerformApplicationRestoreResponse(value: false, errors: [error]);
    }

    public static implicit operator PerformApplicationRestoreResponse(Error[] errors) {
        return new PerformApplicationRestoreResponse(value: false, errors);
    }
}