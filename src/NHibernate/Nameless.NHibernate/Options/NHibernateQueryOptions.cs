using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public sealed class NHibernateQueryOptions : NHibernateOptionsBase {
        #region Public Properties

        [Description("query.substitutions")]
        public string? Substitutions { get; set; } = "true=1;false=0";

        [Description("query.default_cast_length")]
        public int? DefaultCastLength { get; set; }

        [Description("query.default_cast_precision")]
        public int? DefaultCastPrecision { get; set; }

        [Description("query.default_cast_scale")]
        public int? DefaultCastScale { get; set; }

        [Description("query.startup_check")]
        public bool? StartupCheck { get; set; }

        [Description("query.factory_class")]
        public string? FactoryClass { get; set; }

        [Description("query.linq_provider_class")]
        public string? LinqProviderClass { get; set; }

        [Description("query.query_model_rewriter_factory")]
        public string? QueryModelRewriterFactory { get; set; }

        #endregion
    }
}
