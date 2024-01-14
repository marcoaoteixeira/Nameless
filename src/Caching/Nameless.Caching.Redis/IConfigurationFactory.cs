using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public interface IConfigurationFactory {
        #region Methods

        ConfigurationOptions CreateConfigurationOptions();

        #endregion
    }
}
