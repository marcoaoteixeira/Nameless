using MailKit.Net.Smtp;
using System.Diagnostics.CodeAnalysis;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Mailing.Mailkit;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Internal)]
internal static class SmtpClientExtensions {
    internal static Task AuthenticateSmtpClientAsync(this SmtpClient client, MailingOptions options, CancellationToken cancellationToken) {
        if (!client.Capabilities.HasFlag(SmtpCapabilities.Authentication) || !options.UseCredentials) {
            return Task.CompletedTask;
        }

        client.AuthenticationMechanisms.Remove(item: "XOAUTH2");

        return client.AuthenticateAsync(options.Username, options.Password, cancellationToken);
    }
}