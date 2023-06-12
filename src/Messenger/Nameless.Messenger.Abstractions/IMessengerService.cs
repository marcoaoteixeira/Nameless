namespace Nameless.Messenger {

    public interface IMessengerService {

		#region Methods

		Task<MessageResponse> DispatchAsync(MessageRequest request, CancellationToken cancellationToken = default);

		#endregion
	}
}
