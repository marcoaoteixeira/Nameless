using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public sealed record NHibernateAdoNetOptions : NHibernateOptionsBase {
        #region Public Properties

        [Description("adonet.batch_size")]
        public int? BatchSize { get; set; }

        [Description("adonet.batch_versioned_data")]
        public bool? BatchVersionedData { get; set; }

        [Description("adonet.factory_class")]
        public string? FactoryClass { get; set; }

        [Description("adonet.wrap_result_sets")]
        public bool? WrapResultSets { get; set; }

        #endregion
    }
}
