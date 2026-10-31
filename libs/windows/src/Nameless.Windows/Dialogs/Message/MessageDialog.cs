using Nameless.Helpers;
using Nameless.Windows.Dialogs.Message.Resources;

namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     Message Dialog
/// </summary>
public class MessageDialog : IMessageDialog {
    /// <inheritdoc/>
    public MessageDialogResult Show(string message, Action<MessageDialogOptions> configure) {
        var options = ActionHelper.FromDelegate(configure);

        SysMessageBoxResult result;

        if (options.Owner is null) {
            result = SysMessageBox.Show(
                messageBoxText: message,
                caption: options.Title ?? MessageDialogResources.Show_FallbackTitle,
                button: options.Buttons.ToSystem(),
                icon: options.Icon.ToSystem()
            );
        }
        else {
            result = SysMessageBox.Show(
                owner: options.Owner,
                messageBoxText: message,
                caption: options.Title ?? MessageDialogResources.Show_FallbackTitle,
                button: options.Buttons.ToSystem(),
                icon: options.Icon.ToSystem()
            );
        }

        return result.ToMessageDialogResult();
    }
}