using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Nameless.Windows.Localization;

/// <summary>
///     Multi-value converter used by <see cref="L10NExtension"/> when
///     <see cref="L10NExtension.Parameters"/> is set.
///     Expects <c>values[0]</c> to be the translated format string and
///     <c>values[1]</c> to be either a scalar or a non-string
///     <see cref="IEnumerable"/> whose items are spread as positional
///     arguments into <see cref="string.Format(string,object[])"/>.
/// </summary>
internal sealed class LocalizerFormatConverter : IMultiValueConverter {
    /// <summary>Gets the singleton instance.</summary>
    internal static readonly LocalizerFormatConverter Instance = new();

    private LocalizerFormatConverter() { }

    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        if (values.Length < 2 || values[0] is not string format) {
            return DependencyProperty.UnsetValue;
        }

        var args = values[1] is not string && values[1] is IEnumerable enumerable
            ? enumerable.Cast<object>().ToArray()
            : (object[])[values[1]];

        return string.Format(format, args);
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotSupportedException();
    }
}
