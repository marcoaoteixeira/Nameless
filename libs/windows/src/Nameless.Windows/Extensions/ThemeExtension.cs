using System.Windows.Controls;
using Nameless.Windows.Helpers;
using Nameless.Windows.UI;
using Wpf.Ui.Appearance;

namespace Nameless.Windows;

/// <summary>
///     Theme Extensions Methods
/// </summary>
public static class ThemeExtension {
    /// <param name="self">
    ///     The current <see cref="Theme"/> instance.
    /// </param>
    extension(Theme self) {
        /// <summary>
        ///     Converts the current <see cref="Theme"/> instance into
        ///     an <see cref="ApplicationTheme"/> instance.
        /// </summary>
        /// <returns>
        ///     An <see cref="ApplicationTheme"/> instance.
        /// </returns>
        public ApplicationTheme ToApplicationTheme() {
            return self switch {
                Theme.Light => ApplicationTheme.Light,
                Theme.Dark => ApplicationTheme.Dark,
                Theme.HighContrast => ApplicationTheme.HighContrast,
                _ => ApplicationTheme.Unknown
            };
        }

        /// <summary>
        ///     Converts the current <see cref="Theme"/> instance into
        ///     an <see cref="ComboBoxItem"/> instance.
        /// </summary>
        /// <returns>
        ///     An <see cref="ComboBoxItem"/> instance.
        /// </returns>
        public ComboBoxItem ToComboBoxItem() {
            return ComboBoxItemHelper.Create(self, self.DisplayText);
        }

        /// <summary>
        ///     Finds the current <see cref="Theme"/> instance in
        ///     the available collection of <see cref="ComboBoxItem"/> instances.
        /// </summary>
        /// <param name="available">
        ///     The available <see cref="ComboBoxItem"/> instances.
        /// </param>
        /// <returns>
        ///     The found <see cref="ComboBoxItem"/> instance or and
        ///     empty instance.
        /// </returns>
        public ComboBoxItem Find(ComboBoxItem[] available) {
            return ComboBoxItemHelper.TryGet(available, self, out var output)
                ? output
                : ComboBoxItemHelper.EmptyComboBoxItem;
        }
    }
}