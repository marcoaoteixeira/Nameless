namespace Nameless.Windows.Dialogs.Message;

public static class MessageDialogIconExtensions {
    extension(MessageDialogIcon self) {
        public SysMessageBoxImage ToSystem() {
            return self switch {
                MessageDialogIcon.Warning => SysMessageBoxImage.Warning,
                MessageDialogIcon.Error => SysMessageBoxImage.Error,
                MessageDialogIcon.Attention => SysMessageBoxImage.Exclamation,
                MessageDialogIcon.Question => SysMessageBoxImage.Question,
                _ => SysMessageBoxImage.Information,
            };
        }
    }
}