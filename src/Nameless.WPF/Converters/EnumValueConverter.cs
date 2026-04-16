using System.Globalization;
using System.Windows.Data;

namespace Nameless.WPF.Converters;

/// <summary>
///     Converts enum values to and from their string representations.
/// </summary>
/// <typeparam name="TEnum">
///     Type of the enum.
/// </typeparam>
public abstract class EnumValueConverter<TEnum> : IValueConverter
    where TEnum : struct, Enum {
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value is TEnum enumValue
            ? enumValue.ToString()
            : string.Empty;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value is string enumValue && Enum.TryParse<TEnum>(enumValue, out var result)
            ? result
            : default;
    }
}