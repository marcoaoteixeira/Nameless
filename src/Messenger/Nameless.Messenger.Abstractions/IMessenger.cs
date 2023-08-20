namespace Nameless.Messenger {
    public interface IMessenger {
        #region Methods

        Task<Response> SendAsync(Request request, CancellationToken cancellationToken = default);

        #endregion
    }
}
