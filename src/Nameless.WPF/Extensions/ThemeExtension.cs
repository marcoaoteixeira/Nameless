using System.Windows.Controls;
using Nameless.WPF.Helpers;
using Nameless.WPF.UI;
using Wpf.Ui.Appearance;

namespace Nameless.WPF;

public static class ThemeExtension {
    extension(Theme self) {
        public ApplicationTheme ToApplicationTheme() {
            return self switch {
                Theme.Light => ApplicationTheme.Light,
                Theme.Dark => ApplicationTheme.Dark,
                Theme.HighContrast => ApplicationTheme.HighContrast,
                _ => ApplicationTheme.Light
            };
        }

        public ComboBoxItem ToComboBoxItem() {
            return ComboBoxItemHelper.Create(self, self.DisplayText);
        }

        public ComboBoxItem GetComboBoxItem(ComboBoxItem[] available) {
            return ComboBoxItemHelper.TrySelect(available, self, out var output)
                ? output
                : ComboBoxItemHelper.EmptyComboBoxItem;
        }
    }
}