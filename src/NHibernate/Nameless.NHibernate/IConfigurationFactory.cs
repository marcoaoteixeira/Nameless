using NHibernate.Cfg;

namespace Nameless.NHibernate {
    public interface IConfigurationFactory {
        #region Methods

        Configuration CreateConfiguration();

        #endregion
    }
}
