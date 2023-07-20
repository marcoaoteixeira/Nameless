using NHibernate.Cfg;

namespace Nameless.NHibernate.Services {
    public interface IConfigurationBuilder {
        #region Methods

        Configuration Build();

        #endregion
    }
}
