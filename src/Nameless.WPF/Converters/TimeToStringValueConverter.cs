using System.Globalization;
using System.Windows.Data;

namespace Nameless.WPF.Converters;

/// <summary>
///     Converts a <see cref="string"/> value into a <see cref="TimeSpan"/> or
///     <see cref="TimeOnly"/> given the target type.
///     Works also in the opposite direction.
/// </summary>
public class TimeToStringValueConverter : IValueConverter {
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            TimeSpan timespan => string.Format(culture, "{0:g}", timespan),
            TimeOnly timeOnly => string.Format(culture, "{0:g}", timeOnly),
            _ => string.Empty
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        var date = value as string;

        if (typeof(TimeSpan).IsAssignableFrom(targetType)) {
            return TimeSpan.TryParse(date, culture, out var output) ? output : null;
        }

        if (typeof(TimeOnly).IsAssignableFrom(targetType)) {
            return TimeOnly.TryParse(date, culture, out var output) ? output : null;
        }

        return null;
    }
}