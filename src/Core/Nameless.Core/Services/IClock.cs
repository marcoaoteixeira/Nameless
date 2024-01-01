namespace Nameless.Services {
    public interface IClock {
        #region Properties

        DateTime GetUtcNow();
        DateTimeOffset GetUtcNowOffset();

        #endregion
    }
}
