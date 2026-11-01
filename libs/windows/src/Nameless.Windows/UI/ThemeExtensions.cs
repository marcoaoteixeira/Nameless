using Nameless.Windows.UI.Resources;

namespace Nameless.Windows.UI;

/// <summary>
///     Theme Extension Methods
/// </summary>
public static class ThemeExtensions {
    extension(Theme self) {
        /// <summary>
        ///     Gets the display text for the specified theme.
        /// </summary>
        public string DisplayText => self switch {
            Theme.System => ThemeResources.DisplayText_System,
            Theme.Light => ThemeResources.DisplayText_Light,
            Theme.Dark => ThemeResources.DisplayText_Dark,
            Theme.HighContrast => ThemeResources.DisplayText_HighContrast,
            _ => self.ToString()
        };
    }
}
