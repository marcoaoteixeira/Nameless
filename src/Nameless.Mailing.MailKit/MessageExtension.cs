using MimeKit;
using MimeKit.Text;

namespace Nameless.Mailing.MailKit;

/// <summary>
/// <see cref="Message"/> extension methods for MailKit.
/// </summary>
internal static class MessageExtension {
    /// <summary>
    /// Converts a <see cref="Message"/> to a <see cref="MimeMessage"/>.
    /// </summary>
    /// <param name="self">The current message.</param>
    /// <returns>
    /// A <see cref="MimeMessage"/> representation of the current message.
    /// </returns>
    internal static MimeMessage ToMimeMessage(this Message self) {
        var format = self.IsBodyHtml
            ? TextFormat.Html
            : TextFormat.Plain;

        return new MimeMessage {
            Body = new TextPart(format) { Text = self.Content },
            Sender = MailboxAddress.Parse(self.From[0]),
            Subject = self.Subject,
            Priority = self.Priority switch {
                Priority.Low => MessagePriority.NonUrgent,
                Priority.Normal => MessagePriority.Normal,
                Priority.High => MessagePriority.Urgent,
                _ => MessagePriority.Normal
            }
        };
    }
}