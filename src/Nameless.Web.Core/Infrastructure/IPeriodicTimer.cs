namespace Nameless.Web.Infrastructure;

public interface IPeriodicTimer : IDisposable {
    TimeSpan Period { get; set; }
    ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken);
}