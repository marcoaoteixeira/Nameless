namespace Nameless.Windows.Dialogs.Message;

public static class MessageDialogButtonsExtensions {
    extension(MessageDialogButtons self) {
        public SysMessageBoxButton ToSystem() {
            return self switch {
                MessageDialogButtons.OkCancel => SysMessageBoxButton.OKCancel,
                MessageDialogButtons.YesNo => SysMessageBoxButton.YesNo,
                MessageDialogButtons.YesNoCancel => SysMessageBoxButton.YesNoCancel,
                _ => SysMessageBoxButton.OK,
            };
        }
    }
}