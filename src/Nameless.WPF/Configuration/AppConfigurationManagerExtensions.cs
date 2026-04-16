using Nameless.WPF.UI;

namespace Nameless.WPF.Configuration;

public static class AppConfigurationManagerExtensions {
    private const string THEME_KEY = nameof(Theme);
    private const string CONFIRM_BEFORE_EXIT_KEY = "ConfirmBeforeExit";

    extension(IAppConfigurationManager self) {
        public Theme Theme {
            get => self.TryGet<Theme>(THEME_KEY, out var output) ? output : default;
            set => self.Set(THEME_KEY, value);
        }

        public bool ConfirmBeforeExit {
            get => self.TryGet<bool>(CONFIRM_BEFORE_EXIT_KEY, out var output) && output;
            set => self.Set(CONFIRM_BEFORE_EXIT_KEY, value);
        }
    }
}
