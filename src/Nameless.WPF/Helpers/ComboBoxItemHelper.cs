using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using Nameless.WPF.Resources;

namespace Nameless.WPF.Helpers;

public static class ComboBoxItemHelper {
    public static ComboBoxItem EmptyComboBoxItem => new() {
        Content = Strings.ComboBoxItemHelper_EmptyComboBoxItem_Content
    };

    public static ComboBoxItem Create<TEnum>(TEnum value, string? displayText = null)
        where TEnum : struct, Enum {
        return new ComboBoxItem {
            Content = displayText ?? value.GetDescription(),
            Tag = value
        };
    }

    public static bool TrySelect<TEnum>(ComboBoxItem[] items, TEnum value, [NotNullWhen(returnValue: true)] out ComboBoxItem? output)
        where TEnum : struct, Enum {
        output = items.FirstOrDefault(item => Equals(item.Tag, value));

        output?.IsSelected = true;

        return output is not null;
    }
}