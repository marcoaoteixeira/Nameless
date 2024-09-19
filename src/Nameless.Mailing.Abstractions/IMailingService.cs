namespace Nameless.Mailing;

public interface IMailingService {
    Task<string> DispatchAsync(Message message, CancellationToken cancellationToken);
}