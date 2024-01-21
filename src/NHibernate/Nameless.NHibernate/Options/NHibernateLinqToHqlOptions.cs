using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public sealed class NHibernateLinqToHqlOptions : NHibernateOptionsBase {
        #region Public Properties

        [Description("linqtohql.generatorsregistry")]
        public string? GeneratorsRegistry { get; set; }

        #endregion
    }
}
