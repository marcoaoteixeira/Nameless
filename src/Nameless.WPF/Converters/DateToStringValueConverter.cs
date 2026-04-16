using System.Globalization;
using System.Windows.Data;

namespace Nameless.WPF.Converters;

/// <summary>
///     Converts a <see cref="string"/> value into a <see cref="DateTime"/> or
///     <see cref="DateTimeOffset"/> or <see cref="DateOnly"/> given the
///     target type. Works also in the opposite direction.
/// </summary>
public class DateToStringValueConverter : IValueConverter {
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value switch {
            DateTime dateTime => string.Format(culture, "{0:G}", dateTime),
            DateTimeOffset dateTimeOffset => string.Format(culture, "{0:G}", dateTimeOffset),
            DateOnly dateOnly => string.Format(culture, "{0:d}", dateOnly),
            _ => string.Empty
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        var date = value as string;

        if (typeof(DateTime).IsAssignableFrom(targetType)) {
            return DateTime.TryParse(date, culture, out var output) ? output : null;
        }

        if (typeof(DateTimeOffset).IsAssignableFrom(targetType)) {
            return DateTimeOffset.TryParse(date, culture, out var output) ? output : null;
        }

        if (typeof(DateOnly).IsAssignableFrom(targetType)) {
            return DateOnly.TryParse(date, culture, out var output) ? output : null;
        }

        return null;
    }
}