namespace Nameless.Mailing;

/// <summary>
/// Defines a contract for delivering email messages asynchronously.
/// Implementations of this interface are responsible for sending the specified
/// <see cref="Message"/> using the available infrastructure.
/// </summary>
public interface IMailingService {
    /// <summary>
    /// Delivers a message through the available mailing service interface.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="Task"/> represeting the asynchronous operation.
    /// </returns>
    Task DeliverAsync(Message message, CancellationToken cancellationToken);
}