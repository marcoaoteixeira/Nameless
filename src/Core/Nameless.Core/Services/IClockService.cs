namespace Nameless.Services {
    public interface IClockService {
        #region Properties

        DateTime GetUtcNow();
        DateTimeOffset GetUtcNowOffset();

        #endregion
    }
}
