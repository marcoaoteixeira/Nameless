using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Validation;

/// <summary>
///     Represents the validation execution result.
/// </summary>
public sealed class ValidationResult : Result<Nothing> {
    public static ValidationResult Successful => new(value: Nothing.Value, errors: []);

    private ValidationResult(Nothing value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator ValidationResult(Error error) {
        return new ValidationResult(value: default, errors: [error]);
    }

    public static implicit operator ValidationResult(Error[] errors) {
        return new ValidationResult(value: default, errors);
    }
}