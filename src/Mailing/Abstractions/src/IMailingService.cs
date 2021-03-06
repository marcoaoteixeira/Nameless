using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Mailing {

    /// <summary>
    /// Contract to the e-mail messaging service.
    /// </summary>
    public interface IMailingService {

        #region Methods

        /// <summary>
        /// Sends the e-mail message.
        /// </summary>
        /// <param name="message">The e-mail message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A <see cref="Task" /> representing the method execution.</returns>
        Task SendAsync (Message message, CancellationToken token = default);

        #endregion Methods
    }
}