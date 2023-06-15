using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public interface IRedisDatabaseManager {

        #region Methods

        IDatabase GetDatabase();

        #endregion
    }
}
