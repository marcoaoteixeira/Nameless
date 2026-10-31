using Nameless.Windows.UI;

namespace Nameless.Windows.AppConfiguration;

/// <summary>
///     App Configuration Manager Extension Methods.
/// </summary>
public static class AppConfigurationManagerExtensions {
    private const string THEME_KEY = nameof(Theme);
    private const string CONFIRM_BEFORE_EXIT_KEY = "ConfirmBeforeExit";
    private const string UI_LANGUAGE_KEY = "UILanguage";

    extension(IAppConfigurationManager self) {
        /// <summary>
        ///     Gets or sets the theme.
        /// </summary>
        public Theme Theme {
            get => self.TryGet<Theme>(THEME_KEY, out var output) ? output : default;
            set => self.Set(THEME_KEY, value);
        }

        /// <summary>
        ///     Whether it should ask before exit application.
        /// </summary>
        public bool ConfirmBeforeExit {
            get => self.TryGet<bool>(CONFIRM_BEFORE_EXIT_KEY, out var output) && output;
            set => self.Set(CONFIRM_BEFORE_EXIT_KEY, value);
        }

        /// <summary>
        ///     Gets or sets the current application UI language.
        /// </summary>
        public string Language {
            get => self.TryGet<string>(UI_LANGUAGE_KEY, out var output) ? output : "pt-BR";
            set => self.Set(UI_LANGUAGE_KEY, value);
        }
    }
}
