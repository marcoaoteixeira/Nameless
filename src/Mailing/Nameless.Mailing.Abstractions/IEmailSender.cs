namespace Nameless.Mailing;

public interface IEmailDispatcher {
    Task<string> DispatchAsync(Message message, CancellationToken cancellationToken);
}