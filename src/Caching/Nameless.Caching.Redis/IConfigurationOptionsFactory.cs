using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public interface IConfigurationOptionsFactory {
        #region Methods

        ConfigurationOptions CreateConfigurationOptions();

        #endregion
    }
}
