namespace Nameless.Messenger {
    public interface IMessenger {
        #region Methods

        Task<MessageResponse> SendAsync(MessageRequest request, CancellationToken cancellationToken);

        #endregion
    }
}
