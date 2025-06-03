using MailKit.Net.Smtp;

namespace Nameless.Mailing.MailKit;

/// <summary>
/// Factory for creating instances of <see cref="ISmtpClient"/>.
/// </summary>
public interface ISmtpClientFactory {
    /// <summary>
    /// Asynchronously creates an instance of <see cref="ISmtpClient"/>. 
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result
    /// contains an instance of <see cref="ISmtpClient"/>.
    /// </returns>
    Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken);
}