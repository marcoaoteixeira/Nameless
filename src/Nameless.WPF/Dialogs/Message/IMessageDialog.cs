namespace Nameless.WPF.Dialogs.Message;

public interface IMessageDialog {
    MessageDialogResult Show(string message, Action<MessageDialogOptions> configure);
}