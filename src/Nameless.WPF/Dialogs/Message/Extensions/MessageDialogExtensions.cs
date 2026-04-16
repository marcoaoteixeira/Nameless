using System.Windows;

namespace Nameless.WPF.Dialogs.Message.Extensions;

public static class MessageDialogExtensions {
    extension(IMessageDialog self) {
        public MessageDialogResult ShowInformation(string message, string? title = null, Window? owner = null) {
            return self.InnerShow(message, title, MessageDialogButtons.Ok, MessageDialogIcon.Information, owner);
        }

        public MessageDialogResult ShowWarning(string message, string? title = null, MessageDialogButtons buttons = MessageDialogButtons.Ok, Window? owner = null) {
            return self.InnerShow(message, title, buttons, MessageDialogIcon.Warning, owner);
        }

        public MessageDialogResult ShowError(string message, string? title = null, Window? owner = null) {
            return self.InnerShow(message, title, MessageDialogButtons.Ok, MessageDialogIcon.Error, owner);
        }

        public MessageDialogResult ShowAttention(string message, string? title = null, Window? owner = null) {
            return self.InnerShow(message, title, MessageDialogButtons.Ok, MessageDialogIcon.Attention, owner);
        }

        public MessageDialogResult ShowQuestion(string message, string? title = null, MessageDialogButtons buttons = MessageDialogButtons.YesNo, Window? owner = null) {
            return self.InnerShow(message, title, buttons, MessageDialogIcon.Question, owner);
        }

        private MessageDialogResult InnerShow(string message, string? title, MessageDialogButtons buttons, MessageDialogIcon icon, Window? owner) {
            return self.Show(message, opts => {
                opts.Title = title;
                opts.Buttons = buttons;
                opts.Icon = icon;
                opts.Owner = owner;
            });
        }
    }
}