using System.Windows;

namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     Message Dialog Extension Methods
/// </summary>
public static class MessageDialogExtensions {
    extension(IMessageDialog self) {
        /// <summary>Shows an information message.</summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="owner">The window that owns the message dialog.</param>
        /// <returns>A <see cref="MessageDialogResult"/>. </returns>
        public MessageDialogResult ShowInformation(string message, string? title = null, Window? owner = null) {
            return self.InnerShow(message, title, MessageDialogButtons.Ok, MessageDialogIcon.Information, owner);
        }

        /// <summary>Shows a warning message.</summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The dialog box buttons.</param>
        /// <param name="owner">The window that owns the message dialog.</param>
        /// <returns>A <see cref="MessageDialogResult"/>. </returns>
        public MessageDialogResult ShowWarning(string message, string? title = null, MessageDialogButtons buttons = MessageDialogButtons.Ok, Window? owner = null) {
            return self.InnerShow(message, title, buttons, MessageDialogIcon.Warning, owner);
        }

        /// <summary>Shows an error message.</summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="owner">The window that owns the message dialog.</param>
        /// <returns>A <see cref="MessageDialogResult"/>. </returns>
        public MessageDialogResult ShowError(string message, string? title = null, Window? owner = null) {
            return self.InnerShow(message, title, MessageDialogButtons.Ok, MessageDialogIcon.Error, owner);
        }

        /// <summary>Shows an attention message.</summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="owner">The window that owns the message dialog.</param>
        /// <returns>A <see cref="MessageDialogResult"/>. </returns>
        public MessageDialogResult ShowAttention(string message, string? title = null, Window? owner = null) {
            return self.InnerShow(message, title, MessageDialogButtons.Ok, MessageDialogIcon.Attention, owner);
        }

        /// <summary>Shows a question message.</summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="buttons">The dialog box buttons.</param>
        /// <param name="owner">The window that owns the message dialog.</param>
        /// <returns>A <see cref="MessageDialogResult"/>. </returns>
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