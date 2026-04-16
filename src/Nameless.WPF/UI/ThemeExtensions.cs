using Nameless.WPF.Resources;

namespace Nameless.WPF.UI;

public static class ThemeExtensions {
    extension(Theme self) {
        public string DisplayText {
            get => self switch {
                Theme.Light => Strings.Theme_Light,
                Theme.Dark => Strings.Theme_Dark,
                Theme.HighContrast => Strings.Theme_HighContrast,
                _ => self.ToString()
            };
        }
    }
}
