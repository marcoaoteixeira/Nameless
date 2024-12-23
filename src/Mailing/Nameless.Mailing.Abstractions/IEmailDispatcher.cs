namespace Nameless.Mailing;

public interface IEmailDispatcher {
    Task DispatchAsync(Message message, CancellationToken cancellationToken);
}