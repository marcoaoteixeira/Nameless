namespace Nameless.Services;

public interface ISystemClock {
    DateTime GetUtcNow();

    DateTimeOffset GetUtcNowOffset();
}