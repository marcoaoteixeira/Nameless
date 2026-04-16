using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Nameless.WPF.SnackBar;

public static class SnackBarServiceExtension {
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    extension(ISnackbarService self) {
        public void ShowInformation(string content, string? title = null) {
            self.InnerShow(ControlAppearance.Info, content, title);
        }

        public void ShowSuccess(string content, string? title = null) {
            self.InnerShow(ControlAppearance.Success, content, title);
        }

        public void ShowWarning(string content, string? title = null) {
            self.InnerShow(ControlAppearance.Caution, content, title);
        }

        public void ShowFailure(string content, string? title = null) {
            self.InnerShow(ControlAppearance.Danger, content, title);
        }

        public void Show(SnackBarArgs args) {
            self.InnerShow(args.Appearance, args.Content, args.Title);
        }

        private void InnerShow(ControlAppearance appearance, string content, string? title = null) {
            self.Show(title ?? appearance.Title, content, appearance, appearance.Icon, Timeout);
        }
    }
}