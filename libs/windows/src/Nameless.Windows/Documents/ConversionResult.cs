using Nameless.ObjectModel;

namespace Nameless.Windows.Documents;

/// <summary>
///     Content Result
/// </summary>
public class ConversionResult : Result<string> {
    private ConversionResult(string? value, Error[] errors) : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="string"/>
    ///     into a <see cref="ConversionResult"/>.
    /// </summary>
    /// <param name="value">
    ///     The string value.
    /// </param>
    public static implicit operator ConversionResult(string value) {
        return new ConversionResult(value, errors: []);
    }

    /// <summary>
    ///     Converts an <see cref="Error"/>
    ///     into a <see cref="ConversionResult"/>.
    /// </summary>
    /// <param name="error">
    ///     The error value.
    /// </param>
    public static implicit operator ConversionResult(Error error) {
        return new ConversionResult(value: null, errors: [error]);
    }

    /// <summary>
    ///     Converts a collection of <see cref="Error"/>
    ///     into a <see cref="ConversionResult"/>.
    /// </summary>
    /// <param name="errors">
    ///     The error collection.
    /// </param>
    public static implicit operator ConversionResult(Error[] errors) {
        return new ConversionResult(value: null, errors);
    }
}