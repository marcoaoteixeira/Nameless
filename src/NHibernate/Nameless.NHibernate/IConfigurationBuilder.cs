using NHibernate.Cfg;

namespace Nameless.NHibernate {

    public interface IConfigurationBuilder {

        #region Methods

        Configuration Build();

        #endregion
    }
}
