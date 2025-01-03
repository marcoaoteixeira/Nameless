﻿using MimeKit;
using MimeKit.Text;

namespace Nameless.Mailing.MailKit;

public static class MessageExtension {
    public static MimeMessage ToMimeMessage(this Message self) {
        var format = self.IsBodyHtml
            ? TextFormat.Html
            : TextFormat.Plain;

        return new MimeMessage {
            Body = new TextPart(format) {
                Text = self.Content
            },
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