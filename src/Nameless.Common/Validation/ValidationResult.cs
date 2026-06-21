using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Validation;

/// <summary>
///     Represents the validation execution result.
/// </summary>
public sealed class ValidationResult : Result<Nothing> {
    /// <summary>
    ///     Gets a successful validation result instance.
    /// </summary>
    public static ValidationResult Successful => new(value: Nothing.Value, errors: []);

    private ValidationResult(Nothing value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts an <see cref="Error"/> into a
    ///     <see cref="ValidationResult"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator ValidationResult(Error error) {
        return new ValidationResult(value: default, errors: [error]);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="ValidationResult"/> instance.
    /// </summary>
    /// <param name="errors">
    ///     The errors.
    /// </param>
    public static implicit operator ValidationResult(Error[] errors) {
        return new ValidationResult(value: default, errors);
    }
}