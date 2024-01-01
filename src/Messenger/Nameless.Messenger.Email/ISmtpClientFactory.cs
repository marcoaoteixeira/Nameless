using MailKit.Net.Smtp;

namespace Nameless.Messenger.Email {
    public interface ISmtpClientFactory {
        #region Methods

        Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
