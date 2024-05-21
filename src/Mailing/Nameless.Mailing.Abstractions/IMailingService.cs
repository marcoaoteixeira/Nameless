namespace Nameless.Mailing {
    public interface IMailingService {
        #region Methods

        Task<string> DispatchAsync(Message message, CancellationToken cancellationToken);

        #endregion
    }
}
