using MailKit.Net.Smtp;

namespace Nameless.Mailing.MailKit;

public interface ISmtpClientFactory {
    Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken);
}