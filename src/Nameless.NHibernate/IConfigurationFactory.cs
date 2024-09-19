using NHibernate.Cfg;

namespace Nameless.NHibernate;

public interface IConfigurationFactory {
    Configuration CreateConfiguration();
}