using Nameless.WPF.Dialogs.Message.Extensions;

namespace Nameless.WPF.Dialogs.Message.Impl;

public class MessageDialog : IMessageDialog {
    public MessageDialogResult Show(string message, Action<MessageDialogOptions> configure) {
        var options = new MessageDialogOptions();

        configure(options);

        SysMessageBoxResult result;
        if (options.Owner is null) {
            result = SysMessageBox.Show(
                messageBoxText: message,
                caption: options.Title ?? options.Icon.AlternativeText,
                button: options.Buttons.ToSystem(),
                icon: options.Icon.ToSystem()
            );
        }
        else {
            result = SysMessageBox.Show(
                owner: options.Owner,
                messageBoxText: message,
                caption: options.Title ?? options.Icon.AlternativeText,
                button: options.Buttons.ToSystem(),
                icon: options.Icon.ToSystem()
            );
        }

        return result.FromSystem();
    }
}