using Nameless.NHibernate.Options;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate.Impl {
    public sealed class ConfigurationFactory : IConfigurationFactory {
        #region Private Read-Only Fields

        private readonly NHibernateOptions _options;

        #endregion

        #region Public Constructors

        public ConfigurationFactory(NHibernateOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region IConfigurationFactory Members

        public Configuration CreateConfiguration() {
            var configuration = new Configuration();
            configuration.SetProperties(_options.ToDictionary());

            var modelInspector = new ModelInspector(_options.RootEntityTypes);
            var modelMapper = new ModelMapper(modelInspector);

            modelMapper.AddMappings(_options.ClassMappingTypes);

            var mappingDocument = modelMapper
                .CompileMappingForAllExplicitlyAddedEntities();

            configuration.AddDeserializedMapping(mappingDocument: mappingDocument,
                                                 documentFileName: null);

            return configuration;
        }

        #endregion
    }
}
