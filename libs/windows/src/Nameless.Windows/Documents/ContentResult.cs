using Nameless.ObjectModel;

namespace Nameless.Windows.Documents;

/// <summary>
///     Content Result
/// </summary>
public class ContentResult : Result<string> {
    private ContentResult(string? value, Error[] errors) : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="string"/>
    ///     into a <see cref="ContentResult"/>.
    /// </summary>
    /// <param name="value">
    ///     The string value.
    /// </param>
    public static implicit operator ContentResult(string value) {
        return new ContentResult(value, errors: []);
    }

    /// <summary>
    ///     Converts an <see cref="Error"/>
    ///     into a <see cref="ContentResult"/>.
    /// </summary>
    /// <param name="error">
    ///     The error value.
    /// </param>
    public static implicit operator ContentResult(Error error) {
        return new ContentResult(value: null, errors: [error]);
    }

    /// <summary>
    ///     Converts a collection of <see cref="Error"/>
    ///     into a <see cref="ContentResult"/>.
    /// </summary>
    /// <param name="errors">
    ///     The error collection.
    /// </param>
    public static implicit operator ContentResult(Error[] errors) {
        return new ContentResult(value: null, errors);
    }
}