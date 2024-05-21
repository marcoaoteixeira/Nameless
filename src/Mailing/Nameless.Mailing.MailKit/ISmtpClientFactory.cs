using MailKit.Net.Smtp;

namespace Nameless.Mailing.MailKit {
    public interface ISmtpClientFactory {
        #region Methods

        Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken);

        #endregion
    }
}
