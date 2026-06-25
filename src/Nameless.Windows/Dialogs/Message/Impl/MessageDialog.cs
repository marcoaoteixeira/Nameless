using Nameless.Windows.Localization;

namespace Nameless.Windows.Dialogs.Message.Impl;

public class MessageDialog : IMessageDialog {
    private const string CLASS = nameof(MessageDialog);

    private ILocalizer T { get; }

    public MessageDialog(ILocalizer localizer) {
        T = localizer;
    }

    public MessageDialogResult Show(string message, Action<MessageDialogOptions> configure) {
        const string ActionName = nameof(Show);

        var options = new MessageDialogOptions();
        var fallbackTitle = T[$"{CLASS}_{ActionName}_{options.Icon}_Title"];

        configure(options);

        SysMessageBoxResult result;
        if (options.Owner is null) {
            result = SysMessageBox.Show(
                messageBoxText: message,
                caption: options.Title ?? fallbackTitle,
                button: options.Buttons.ToSystem(),
                icon: options.Icon.ToSystem()
            );
        }
        else {
            result = SysMessageBox.Show(
                owner: options.Owner,
                messageBoxText: message,
                caption: options.Title ?? fallbackTitle,
                button: options.Buttons.ToSystem(),
                icon: options.Icon.ToSystem()
            );
        }

        return result.FromSystem();
    }
}