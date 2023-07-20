namespace Nameless.Services {
    public interface IClockService {
        #region Properties

        DateTime UtcNow { get; }
        DateTimeOffset OffsetUtcNow { get; }

        #endregion
    }
}
