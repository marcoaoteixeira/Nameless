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
            Theme.System => nameof(Theme.System),
            Theme.Light => nameof(Theme.Light),
            Theme.Dark => nameof(Theme.Dark),
            Theme.HighContrast => nameof(Theme.HighContrast),
            _ => self.ToString()
        };
    }
}
