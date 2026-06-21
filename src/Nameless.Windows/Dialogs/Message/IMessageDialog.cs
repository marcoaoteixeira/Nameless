namespace Nameless.Windows.Dialogs.Message;

public interface IMessageDialog {
    MessageDialogResult Show(string message, Action<MessageDialogOptions> configure);
}