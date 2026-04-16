namespace Nameless.WPF.Dialogs.Message;

public static class SysMessageBoxResultExtensions {
    extension(SysMessageBoxResult self) {
        public MessageDialogResult FromSystem() {
            return self switch {
                SysMessageBoxResult.Cancel => MessageDialogResult.Cancel,
                SysMessageBoxResult.Yes => MessageDialogResult.Yes,
                SysMessageBoxResult.No => MessageDialogResult.No,
                _ => MessageDialogResult.Ok,
            };
        }
    }
}