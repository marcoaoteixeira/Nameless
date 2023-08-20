using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public interface IConnectionMultiplexerManager {
        #region Methods

        IConnectionMultiplexer GetMultiplexer();

        #endregion
    }
}
