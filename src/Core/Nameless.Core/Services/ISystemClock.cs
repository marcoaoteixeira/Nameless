namespace Nameless.Services {
    public interface ISystemClock {
        #region Properties

        DateTime GetUtcNow();
        DateTimeOffset GetUtcNowOffset();

        #endregion
    }
}
