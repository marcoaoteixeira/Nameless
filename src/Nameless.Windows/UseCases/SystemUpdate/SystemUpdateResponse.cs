using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Windows.UseCases.SystemUpdate;

/// <summary>
///     System update response object
/// </summary>
public class SystemUpdateResponse : Result<SystemUpdateMetadata> {
    private SystemUpdateResponse(SystemUpdateMetadata value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="SystemUpdateMetadata"/> object instance
    ///     into a <see cref="SystemUpdateResponse"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="SystemUpdateMetadata"/> object instance.
    /// </param>
    public static implicit operator SystemUpdateResponse(SystemUpdateMetadata value) {
        return new SystemUpdateResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> object instance
    ///     into a <see cref="SystemUpdateResponse"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> object instance.
    /// </param>
    public static implicit operator SystemUpdateResponse(Error error) {
        return new SystemUpdateResponse(value: default, errors: [error]);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> array instance
    ///     into a <see cref="SystemUpdateResponse"/> instance.
    /// </summary>
    /// <param name="errors">
    ///     The <see cref="Error"/> array instance.
    /// </param>
    public static implicit operator SystemUpdateResponse(Error[] errors) {
        return new SystemUpdateResponse(value: default, errors);
    }
}