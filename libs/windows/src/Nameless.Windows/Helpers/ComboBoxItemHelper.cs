using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace Nameless.Windows.Helpers;

/// <summary>
///     Combo Box Item Helper.
/// </summary>
public static class ComboBoxItemHelper {
    /// <summary>
    ///     Gets an empty combo box item.
    /// </summary>
    public static ComboBoxItem EmptyComboBoxItem => new() {
        Content = string.Empty
    };

    /// <summary>
    ///     Creates a combo box item.
    /// </summary>
    /// <param name="value">
    ///     The combo box item value.
    /// </param>
    /// <param name="displayText">
    ///     The combo box item display text.
    /// </param>
    /// <returns>
    ///     The combo box item.
    /// </returns>
    public static ComboBoxItem Create(string value, string? displayText = null) {
        return new ComboBoxItem {
            Content = displayText ?? value,
            Tag = value
        };
    }

    /// <summary>
    ///     Creates a combo box item for a given enum.
    /// </summary>
    /// <param name="value">
    ///     The combo box item value.
    /// </param>
    /// <param name="displayText">
    ///     The combo box item display text.
    /// </param>
    /// <returns>
    ///     The combo box item.
    /// </returns>
    public static ComboBoxItem Create<TEnum>(TEnum value, string? displayText = null)
        where TEnum : struct, Enum {
        return new ComboBoxItem {
            Content = displayText ?? value.GetDescription(),
            Tag = value
        };
    }

    /// <summary>
    ///     Tries retrieve the associated combo box item to the enum value.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     Type of the enum.
    /// </typeparam>
    /// <param name="items">
    ///     A collection of combo box items.
    /// </param>
    /// <param name="value">
    ///     The enum value.
    /// </param>
    /// <param name="output">
    ///     If found, the combo box item.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if found; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGet<TEnum>(ComboBoxItem[] items, TEnum value, [NotNullWhen(returnValue: true)] out ComboBoxItem? output) where TEnum : struct, Enum {
        output = items.FirstOrDefault(
            item => Enum.IsDefined(typeof(TEnum), item.Tag) && ((TEnum)item.Tag).Equals(value)
        );

        output?.IsSelected = true;

        return output is not null;
    }
}