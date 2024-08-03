using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public sealed record NHibernateSpecificOptions : NHibernateOptionsBase {
        #region Public Properties

        [Description("firebird.disable_parameter_casting")]
        public bool? FirebirdDisableParameterCasting { get; set; }

        [Description("oracle.use_n_prefixed_types_for_unicode")]
        public bool? OracleUseNPrefixedTypesForUnicode { get; set; }

        [Description("odbc.explicit_datetime_scale")]
        public int? OdbcExplicitDateTimeScale { get; set; }

        #endregion
    }
}
