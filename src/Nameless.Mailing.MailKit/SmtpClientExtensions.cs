using MailKit.Net.Smtp;

namespace Nameless.Mailing.MailKit;

internal static class SmtpClientExtensions {
    internal static Task AuthenticateSmtpClientAsync(this SmtpClient client, MailingOptions options,
        CancellationToken cancellationToken) {
        if (!client.Capabilities.HasFlag(SmtpCapabilities.Authentication) || !options.UseCredentials) {
            return Task.CompletedTask;
        }

        client.AuthenticationMechanisms.Remove(item: "XOAUTH2");

        return client.AuthenticateAsync(options.Username, options.Password, cancellationToken);
    }
}