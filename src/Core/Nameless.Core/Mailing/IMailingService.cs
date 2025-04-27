namespace Nameless.Mailing;

public interface IMailingService {
    Task SendAsync(Message message, CancellationToken cancellationToken);
}