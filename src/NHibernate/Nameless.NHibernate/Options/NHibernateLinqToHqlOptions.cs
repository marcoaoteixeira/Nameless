using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public sealed record NHibernateLinqToHqlOptions : NHibernateOptionsBase {
        #region Public Properties

        [Description("linqtohql.generatorsregistry")]
        public string? GeneratorsRegistry { get; set; }

        #endregion
    }
}
