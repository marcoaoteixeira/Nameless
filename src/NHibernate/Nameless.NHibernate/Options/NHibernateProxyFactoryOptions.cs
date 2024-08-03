using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public sealed record NHibernateProxyFactoryOptions : NHibernateOptionsBase {
        #region Public Properties

        [Description("proxyfactory.factory_class")]
        public string? FactoryClass { get; set; }

        #endregion
    }
}
