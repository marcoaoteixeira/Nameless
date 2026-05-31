using System.Globalization;

namespace Nameless.Web.Http.Endpoints.Attributes.Versioning;

/// <summary>
///     Represents the sunset policy for an API endpoint, as defined by
///     <see href="https://www.rfc-editor.org/rfc/rfc8594">RFC 8594</see>.
/// </summary>
/// <remarks>
///     The sunset date signals when the endpoint will become unresponsive.
///     Consumers should migrate to a newer version before this date.
/// </remarks>
public record SunsetMetadata {
    /// <summary>
    ///     Gets the date and time after which the endpoint is considered
    ///     sunset.
    /// </summary>
    public DateTimeOffset SunsetDate { get; }

    /// <summary>
    ///     Initializes a new instance with an explicit
    ///     <see cref="DateTimeOffset"/> value.
    /// </summary>
    /// <param name="sunsetDate">
    ///     The sunset date and time, including UTC offset.
    /// </param>
    public SunsetMetadata(DateTimeOffset sunsetDate) {
        SunsetDate = sunsetDate;
    }

    /// <summary>
    ///     Initializes a new instance by parsing a date string using the
    ///     current culture.
    /// </summary>
    /// <param name="sunsetDate">
    ///     A date/time string parseable by
    ///     <see cref="DateTimeOffset.Parse(string, IFormatProvider)"/>.
    /// </param>
    /// <exception cref="FormatException">
    ///     Thrown when <paramref name="sunsetDate"/> is not a valid date/time
    ///     string.
    /// </exception>
    public SunsetMetadata(string sunsetDate) {
        SunsetDate = DateTimeOffset.Parse(sunsetDate, CultureInfo.CurrentCulture);
    }
}
