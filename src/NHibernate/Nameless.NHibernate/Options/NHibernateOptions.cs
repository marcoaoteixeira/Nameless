using System.Reflection;

namespace Nameless.NHibernate.Options {
    public sealed record NHibernateOptions {
        #region Public Static Read-Only Properties

        public static NHibernateOptions Default => new();

        #endregion

        #region Public Properties

        public NHibernateSchemaExportOptions SchemaExport { get; set; } = new();
        public NHibernateCommonOptions Common { get; set; } = new();
        public NHibernateConnectionOptions Connection { get; set; } = new();
        public NHibernateAdoNetOptions AdoNet { get; set; } = new();
        public NHibernateCacheOptions Cache { get; set; } = new();
        public NHibernateQueryOptions Query { get; set; } = new();
        public NHibernateLinqToHqlOptions LinqToHql { get; set; } = new();
        public NHibernateHbmToDdlOptions HbmToDdl { get; set; } = new();
        public NHibernateProxyFactoryOptions ProxyFactory { get; set; } = new();
        public NHibernateCollectionTypeOptions CollectionType { get; set; } = new();
        public NHibernateTransactionOptions Transaction { get; set; } = new();
        public NHibernateSpecificOptions Specific { get; set; } = new();
        
        #endregion

        #region Public Methods

        public Dictionary<string, string> ToDictionary() {
            var configs = new List<KeyValuePair<string, string>>();

            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(property => typeof(NHibernateOptionsBase).IsAssignableFrom(property.PropertyType));

            foreach (var property in properties) {
                if (property.GetValue(this) is not NHibernateOptionsBase options) {
                    continue;
                }

                configs.AddRange(options.GetConfigValues());
            }

            return new Dictionary<string, string>([.. configs]);
        }

        #endregion
    }
}
